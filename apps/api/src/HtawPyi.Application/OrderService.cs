using HtawPyi.Domain;

namespace HtawPyi.Application;

public class OrderService(
    ITicketRepository tickets,
    IOrderRepository orders,
    IPaymentRepository payments,
    IDrawResultRepository drawResults,
    IGloClient glo,
    IPaymentProvider paymentProvider,
    IUnitOfWork uow,
    TimeProvider clock)
{
    public static readonly TimeSpan ReservationWindow = TimeSpan.FromMinutes(15);
    private const int MaxTicketsPerOrder = 10;

    public async Task<CreateOrderResponse> CreateAsync(
        Guid userId, CreateOrderRequest request, CancellationToken ct = default)
    {
        var ids = request.TicketIds.Distinct().ToList();
        if (ids.Count is < 1 or > MaxTicketsPerOrder)
            throw new DomainException($"An order must contain 1-{MaxTicketsPerOrder} tickets.");

        var now = clock.GetUtcNow().UtcDateTime;
        var found = await tickets.FindByIdsAsync(ids, ct);
        if (found.Count != ids.Count)
            throw new DomainException("One or more tickets no longer exist.", 409);
        if (found.Any(t => !t.IsPurchasable(now)))
            throw new DomainException("One or more tickets are no longer available.", 409);

        // Auto-release: a ticket bought out of an expired reservation frees
        // its old Pending order (marked Expired, items removed) so the
        // one-sale-per-ticket unique index stays satisfiable.
        var expiredHolders = await orders.FindPendingByTicketIdsAsync(
            found.Where(t => t.Status == TicketStatus.Reserved).Select(t => t.Id).ToList(), ct);
        foreach (var stale in expiredHolders)
        {
            stale.Status = OrderStatus.Expired;
            orders.RemoveItems(stale.Items);
            stale.Items.Clear();
        }

        var order = new Order
        {
            UserId = userId,
            Status = OrderStatus.Pending,
            Total = found.Sum(t => t.Price),
            CreatedAt = now,
            Items = found.Select(t => new OrderItem
            {
                TicketId = t.Id,
                PriceAtPurchase = t.Price
            }).ToList()
        };

        foreach (var ticket in found)
        {
            ticket.Status = TicketStatus.Reserved;
            ticket.ReservedUntil = now + ReservationWindow;
        }

        await orders.AddAsync(order, ct);
        var payment = new Payment
        {
            OrderId = order.Id,
            Order = order,
            Provider = paymentProvider.Name,
            Amount = order.Total,
            Status = PaymentStatus.Initiated,
            CreatedAt = now
        };
        await payments.AddAsync(payment, ct);

        // RowVersion concurrency: a competing order for the same ticket makes
        // SaveChanges throw; the infrastructure maps it to a 409.
        await uow.SaveChangesAsync(ct);

        var initiation = await paymentProvider.InitiateAsync(payment, order, ct);
        if (initiation.ProviderRef is not null)
        {
            payment.ProviderRef = initiation.ProviderRef;
            await uow.SaveChangesAsync(ct);
        }

        return new CreateOrderResponse(order.Id, order.Total, payment.Provider, initiation.RedirectUrl);
    }

    /// <summary>Marks an order paid (mock confirm or verified gateway callback).</summary>
    public async Task CompletePaymentAsync(
        Guid paymentId, bool succeeded, string? rawCallback, CancellationToken ct = default)
    {
        var payment = await payments.FindByIdAsync(paymentId, ct)
            ?? throw new DomainException("Payment not found.", 404);
        if (payment.Status != PaymentStatus.Initiated)
            return; // Idempotent: replays change nothing.

        var order = await orders.FindWithItemsAsync(payment.OrderId, ct)
            ?? throw new DomainException("Order not found.", 404);
        var now = clock.GetUtcNow().UtcDateTime;

        payment.RawCallback = rawCallback;
        if (succeeded && order.Status == OrderStatus.Pending)
        {
            payment.Status = PaymentStatus.Succeeded;
            order.Status = OrderStatus.Paid;
            foreach (var item in order.Items)
            {
                if (item.Ticket is { } ticket)
                {
                    ticket.Status = TicketStatus.Sold;
                    ticket.ReservedUntil = null;
                }
            }
        }
        else
        {
            payment.Status = PaymentStatus.Failed;
            order.Status = OrderStatus.Failed;
            foreach (var item in order.Items)
            {
                if (item.Ticket is { } ticket && ticket.Status == TicketStatus.Reserved)
                {
                    ticket.Status = TicketStatus.Available;
                    ticket.ReservedUntil = null;
                }
            }
        }
        _ = now;
        await uow.SaveChangesAsync(ct);
    }

    public async Task<IReadOnlyList<OrderDto>> ListMineAsync(Guid userId, CancellationToken ct = default)
    {
        var mine = await orders.ListForUserAsync(userId, ct);
        var result = new List<OrderDto>();
        foreach (var order in mine)
        {
            var items = new List<OrderItemDto>();
            foreach (var item in order.Items)
            {
                if (item.Ticket is not { } ticket) continue;
                var wins = order.Status == OrderStatus.Paid
                    ? await WinsForAsync(ticket, ct)
                    : [];
                items.Add(new OrderItemDto(
                    ticket.Number,
                    ticket.DrawDate.ToString("yyyy-MM-dd", System.Globalization.CultureInfo.InvariantCulture),
                    item.PriceAtPurchase,
                    wins));
            }
            result.Add(new OrderDto(
                order.Id, order.Status.ToString(), order.Total, order.CreatedAt, items));
        }
        return result;
    }

    private async Task<List<WinDto>> WinsForAsync(Ticket ticket, CancellationToken ct)
    {
        var today = DateOnly.FromDateTime(clock.GetUtcNow().UtcDateTime);
        if (ticket.DrawDate > today) return [];

        var cached = await drawResults.FindAsync(ticket.DrawDate, ct);
        if (cached is null)
        {
            var json = await glo.FetchResultJsonAsync(ticket.DrawDate, ct);
            if (json is null) return [];
            cached = new DrawResult
            {
                DrawDate = ticket.DrawDate,
                FetchedAt = clock.GetUtcNow().UtcDateTime,
                ResultJson = json
            };
            await drawResults.AddAsync(cached, ct);
            await uow.SaveChangesAsync(ct);
        }

        return TicketMatcher.CheckTicket(cached.ResultJson, ticket.Number)
            .Select(w => new WinDto(w.Name, w.Reward))
            .ToList();
    }
}
