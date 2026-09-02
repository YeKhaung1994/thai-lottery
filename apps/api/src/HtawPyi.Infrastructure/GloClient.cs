using System.Text;
using System.Text.Json;
using HtawPyi.Application;

namespace HtawPyi.Infrastructure;

/// <summary>
/// Server-to-server client for the official GLO API (no CORS constraints
/// here — the browser-facing /glo proxy is a customer-app concern only).
/// </summary>
public class GloClient(HttpClient http) : IGloClient
{
    public const string BaseUrl = "https://www.glo.or.th/api/lottery/";

    public async Task<string?> FetchResultJsonAsync(DateOnly drawDate, CancellationToken ct = default)
    {
        var body = JsonSerializer.Serialize(new
        {
            date = drawDate.Day.ToString("D2"),
            month = drawDate.Month.ToString("D2"),
            year = drawDate.Year.ToString()
        });
        using var response = await http.PostAsync(
            "getLotteryResult",
            new StringContent(body, Encoding.UTF8, "application/json"), ct);
        if (!response.IsSuccessStatusCode) return null;

        using var doc = JsonDocument.Parse(await response.Content.ReadAsStringAsync(ct));
        if (!doc.RootElement.TryGetProperty("response", out var resp) ||
            resp.ValueKind != JsonValueKind.Object ||
            !resp.TryGetProperty("data", out var data) ||
            data.ValueKind != JsonValueKind.Object)
            return null;

        // Store only the prize data object — the shape TicketMatcher reads.
        return data.GetRawText();
    }
}
