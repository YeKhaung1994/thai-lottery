using System.Globalization;
using System.Security.Claims;
using HtawPyi.Application;
using HtawPyi.Domain;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace HtawPyi.Api;

public static class Dates
{
    /// <summary>ISO parse/format, immune to the host's (Thai Buddhist) culture.</summary>
    public static DateOnly? ParseIso(string? value) =>
        DateOnly.TryParseExact(value, "yyyy-MM-dd", CultureInfo.InvariantCulture,
            DateTimeStyles.None, out var d) ? d : null;

    public static string ToIso(this DateOnly date) =>
        date.ToString("yyyy-MM-dd", CultureInfo.InvariantCulture);
}

public static class ClaimsExtensions
{
    public static Guid UserId(this ClaimsPrincipal user) =>
        Guid.Parse(user.FindFirstValue(ClaimTypes.NameIdentifier)
            ?? user.FindFirstValue("sub")
            ?? throw new DomainException("Not authenticated.", 401));
}

[ApiController]
[Route("api/auth")]
public class AuthController(AuthService auth) : ControllerBase
{
    [HttpPost("register")]
    public async Task<ActionResult<AuthResponse>> Register(RegisterRequest request, CancellationToken ct)
        => StatusCode(201, await auth.RegisterAsync(request, ct));

    [HttpPost("login")]
    public async Task<ActionResult<AuthResponse>> Login(LoginRequest request, CancellationToken ct)
        => await auth.LoginAsync(request, ct);

    [HttpPost("refresh")]
    public async Task<ActionResult<AuthResponse>> Refresh(RefreshRequest request, CancellationToken ct)
        => await auth.RefreshAsync(request, ct);
}

[ApiController]
[Route("api/tickets")]
public class TicketsController(ITicketRepository tickets, TimeProvider clock) : ControllerBase
{
    [HttpGet]
    public async Task<ActionResult<IEnumerable<TicketDto>>> Search(
        [FromQuery] string? drawDate, [FromQuery] string? q, CancellationToken ct)
    {
        var found = await tickets.SearchAsync(
            Dates.ParseIso(drawDate), q, clock.GetUtcNow().UtcDateTime, ct);
        return Ok(found.Select(t => new TicketDto(
            t.Id, t.DrawDate.ToIso(), t.Number, t.Price)));
    }
}

[ApiController]
[Route("api/orders")]
[Authorize]
public class OrdersController(OrderService orders) : ControllerBase
{
    [HttpPost]
    public async Task<ActionResult<CreateOrderResponse>> Create(
        CreateOrderRequest request, CancellationToken ct)
        => StatusCode(201, await orders.CreateAsync(User.UserId(), request, ct));

    [HttpGet("mine")]
    public async Task<ActionResult<IEnumerable<OrderDto>>> Mine(CancellationToken ct)
        => Ok(await orders.ListMineAsync(User.UserId(), ct));
}

[ApiController]
[Route("api/payments")]
public class PaymentsController(
    OrderService orders,
    IPaymentRepository payments,
    IPaymentProvider provider,
    PaymentOptionsView options) : ControllerBase
{
    /// <summary>Mock-provider confirmation (dev flow). The signed-in buyer
    /// confirms or cancels a pending mock payment.</summary>
    [HttpPost("{id:guid}/mock-confirm")]
    [Authorize]
    public async Task<IActionResult> MockConfirm(
        Guid id, [FromQuery] bool success, CancellationToken ct)
    {
        if (options.Provider != "Mock")
            return NotFound();
        await orders.CompletePaymentAsync(id, success, "mock-confirm", ct);
        return NoContent();
    }

    /// <summary>Gateway server-to-server callback (2C2P).</summary>
    [HttpPost("callback")]
    public async Task<IActionResult> Callback(CancellationToken ct)
    {
        using var reader = new StreamReader(Request.Body);
        var raw = await reader.ReadToEndAsync(ct);
        var headers = Request.Headers.ToDictionary(
            h => h.Key, h => h.Value.ToString());

        var verified = provider.VerifyCallback(raw, headers);
        if (verified is not { } result)
            return Unauthorized();

        var payment = await payments.FindByProviderRefAsync(
            provider.Name, result.ProviderRef, ct);
        if (payment is null) return NotFound();
        if (payment.Amount != result.Amount)
            return Unauthorized(); // Amount mismatch: reject.

        await orders.CompletePaymentAsync(payment.Id, result.Succeeded, raw, ct);
        return Ok();
    }
}

/// <summary>Read-only view of payment config for controllers.</summary>
public record PaymentOptionsView(string Provider);

[ApiController]
[Route("api/admin")]
[Authorize(Roles = "Admin")]
public class AdminController(AdminTicketService admin) : ControllerBase
{
    [HttpPost("tickets")]
    public async Task<ActionResult<UploadReport>> Upload(
        List<UploadTicketRequest> rows, CancellationToken ct)
        => await admin.UploadAsync(User.UserId(), rows, ct);

    [HttpGet("tickets")]
    public async Task<ActionResult<IEnumerable<AdminTicketDto>>> List(
        [FromQuery] string? drawDate, [FromQuery] string? status, CancellationToken ct)
    {
        TicketStatus? s = Enum.TryParse<TicketStatus>(status, true, out var parsed) ? parsed : null;
        return Ok(await admin.ListAsync(Dates.ParseIso(drawDate), s, ct));
    }

    [HttpDelete("tickets/{id:guid}")]
    public async Task<IActionResult> Delete(Guid id, CancellationToken ct)
    {
        await admin.DeleteAsync(id, ct);
        return NoContent();
    }

    [HttpGet("orders")]
    public async Task<ActionResult<IEnumerable<AdminOrderDto>>> Orders(CancellationToken ct)
        => Ok(await admin.ListOrdersAsync(ct));
}
