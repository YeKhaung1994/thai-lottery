using System.Security.Cryptography;
using System.Text;
using System.Text.Json;
using HtawPyi.Application;
using HtawPyi.Domain;

namespace HtawPyi.Infrastructure;

public class PaymentOptions
{
    /// <summary>"Mock" (default) or "TwoCTwoP".</summary>
    public string Provider { get; set; } = "Mock";
    public TwoCTwoPOptions TwoCTwoP { get; set; } = new();

    /// <summary>Base URL of the customer app, for redirect targets.</summary>
    public string CustomerAppUrl { get; set; } = "http://localhost:8080";
}

public class TwoCTwoPOptions
{
    // Blank by design — set via env (PAYMENT__2C2P__MERCHANT_ID etc.)
    // once real 2C2P credentials exist. The API refuses to start with
    // Provider=TwoCTwoP while these are blank (see Program.cs).
    public string MerchantId { get; set; } = "";
    public string SecretKey { get; set; } = "";
    public string PaymentTokenUrl { get; set; } =
        "https://sandbox-pgw.2c2p.com/payment/4.3/paymentToken";
}

/// <summary>
/// Development provider: the customer app shows a confirm screen and calls
/// POST /api/payments/{id}/mock-confirm to complete the payment.
/// </summary>
public class MockPaymentProvider(PaymentOptions options) : IPaymentProvider
{
    public string Name => "Mock";

    public Task<PaymentInitiation> InitiateAsync(
        Payment payment, Order order, CancellationToken ct = default) =>
        Task.FromResult(new PaymentInitiation(
            $"{options.CustomerAppUrl}/pay/mock/{payment.Id}",
            $"MOCK-{payment.Id:N}"));

    public (string ProviderRef, bool Succeeded, decimal Amount)? VerifyCallback(
        string rawBody, IDictionary<string, string> headers) => null; // Mock has no callbacks.
}

/// <summary>
/// 2C2P PGW structure — complete flow shape, NOT yet verified against the
/// sandbox (credentials are blank until provided). Do not enable in
/// production without running the sandbox happy path first.
/// </summary>
public class TwoCTwoPProvider(PaymentOptions options, HttpClient http) : IPaymentProvider
{
    public string Name => "2C2P";

    public async Task<PaymentInitiation> InitiateAsync(
        Payment payment, Order order, CancellationToken ct = default)
    {
        var invoiceNo = payment.Id.ToString("N")[..20];
        var payload = JsonSerializer.Serialize(new
        {
            merchantID = options.TwoCTwoP.MerchantId,
            invoiceNo,
            description = $"htawpyi order {order.Id:N}",
            amount = payment.Amount,
            currencyCode = "THB",
            frontendReturnUrl = $"{options.CustomerAppUrl}/pay/return"
        });
        var token = Jwt(payload, options.TwoCTwoP.SecretKey);
        using var response = await http.PostAsync(
            options.TwoCTwoP.PaymentTokenUrl,
            new StringContent(JsonSerializer.Serialize(new { payload = token }),
                Encoding.UTF8, "application/json"), ct);
        response.EnsureSuccessStatusCode();

        using var doc = JsonDocument.Parse(await response.Content.ReadAsStringAsync(ct));
        var webPaymentUrl = doc.RootElement.TryGetProperty("webPaymentUrl", out var url)
            ? url.GetString()
            : null;
        return new PaymentInitiation(
            webPaymentUrl ?? throw new DomainException("2C2P did not return a payment URL.", 502),
            invoiceNo);
    }

    public (string ProviderRef, bool Succeeded, decimal Amount)? VerifyCallback(
        string rawBody, IDictionary<string, string> headers)
    {
        try
        {
            using var doc = JsonDocument.Parse(rawBody);
            if (!doc.RootElement.TryGetProperty("payload", out var payloadEl) ||
                payloadEl.GetString() is not { } jwt)
                return null;

            var parts = jwt.Split('.');
            if (parts.Length != 3) return null;
            var expected = Sign($"{parts[0]}.{parts[1]}", options.TwoCTwoP.SecretKey);
            if (!CryptographicOperations.FixedTimeEquals(
                    Encoding.UTF8.GetBytes(expected), Encoding.UTF8.GetBytes(parts[2])))
                return null; // Signature mismatch: reject.

            using var body = JsonDocument.Parse(B64UrlDecode(parts[1]));
            var root = body.RootElement;
            var invoiceNo = root.GetProperty("invoiceNo").GetString()!;
            var respCode = root.GetProperty("respCode").GetString();
            var amount = root.TryGetProperty("amount", out var a) ? a.GetDecimal() : 0m;
            return (invoiceNo, respCode == "0000", amount);
        }
        catch (Exception ex) when (ex is JsonException or KeyNotFoundException or FormatException)
        {
            return null;
        }
    }

    private static string Jwt(string payloadJson, string secret)
    {
        var header = B64Url(Encoding.UTF8.GetBytes("""{"alg":"HS256","typ":"JWT"}"""));
        var payload = B64Url(Encoding.UTF8.GetBytes(payloadJson));
        return $"{header}.{payload}.{Sign($"{header}.{payload}", secret)}";
    }

    private static string Sign(string input, string secret) =>
        B64Url(HMACSHA256.HashData(Encoding.UTF8.GetBytes(secret), Encoding.UTF8.GetBytes(input)));

    private static string B64Url(byte[] bytes) =>
        Convert.ToBase64String(bytes).TrimEnd('=').Replace('+', '-').Replace('/', '_');

    private static byte[] B64UrlDecode(string s)
    {
        var padded = s.Replace('-', '+').Replace('_', '/');
        return Convert.FromBase64String(padded.PadRight((padded.Length + 3) / 4 * 4, '='));
    }
}
