using HtawPyi.Application;
using HtawPyi.Domain;
using Microsoft.EntityFrameworkCore;

namespace HtawPyi.Infrastructure;

public class UserRepository(AppDbContext db) : IUserRepository
{
    public Task<User?> FindByEmailAsync(string email, CancellationToken ct = default) =>
        db.Users.FirstOrDefaultAsync(u => u.Email == email, ct);

    public Task<User?> FindByIdAsync(Guid id, CancellationToken ct = default) =>
        db.Users.FirstOrDefaultAsync(u => u.Id == id, ct);

    public async Task AddAsync(User user, CancellationToken ct = default) =>
        await db.Users.AddAsync(user, ct);
}

public class RefreshTokenRepository(AppDbContext db) : IRefreshTokenRepository
{
    public Task<RefreshToken?> FindByHashAsync(string tokenHash, CancellationToken ct = default) =>
        db.RefreshTokens.Include(t => t.User)
            .FirstOrDefaultAsync(t => t.TokenHash == tokenHash, ct);

    public async Task AddAsync(RefreshToken token, CancellationToken ct = default) =>
        await db.RefreshTokens.AddAsync(token, ct);
}

public class TicketRepository(AppDbContext db) : ITicketRepository
{
    public async Task<IReadOnlyList<Ticket>> SearchAsync(
        DateOnly? drawDate, string? query, DateTime now, CancellationToken ct = default)
    {
        var q = db.Tickets.AsNoTracking()
            .Where(t => t.Status == TicketStatus.Available ||
                        (t.Status == TicketStatus.Reserved && t.ReservedUntil < now));
        if (drawDate is { } d) q = q.Where(t => t.DrawDate == d);
        if (!string.IsNullOrWhiteSpace(query)) q = q.Where(t => t.Number.Contains(query));
        return await q.OrderBy(t => t.Number).Take(200).ToListAsync(ct);
    }

    public async Task<IReadOnlyList<Ticket>> FindByIdsAsync(
        IReadOnlyCollection<Guid> ids, CancellationToken ct = default) =>
        await db.Tickets.Where(t => ids.Contains(t.Id)).ToListAsync(ct);

    public async Task<IReadOnlyList<Ticket>> ListAsync(
        DateOnly? drawDate, TicketStatus? status, CancellationToken ct = default)
    {
        var q = db.Tickets.AsNoTracking().AsQueryable();
        if (drawDate is { } d) q = q.Where(t => t.DrawDate == d);
        if (status is { } s) q = q.Where(t => t.Status == s);
        return await q.OrderByDescending(t => t.DrawDate).ThenBy(t => t.Number)
            .Take(500).ToListAsync(ct);
    }

    public Task<Ticket?> FindByIdAsync(Guid id, CancellationToken ct = default) =>
        db.Tickets.FirstOrDefaultAsync(t => t.Id == id, ct);

    public async Task AddRangeAsync(IEnumerable<Ticket> tickets, CancellationToken ct = default) =>
        await db.Tickets.AddRangeAsync(tickets, ct);

    public Task<bool> ExistsAsync(DateOnly drawDate, string number, CancellationToken ct = default) =>
        db.Tickets.AnyAsync(t => t.DrawDate == drawDate && t.Number == number, ct);

    public void Remove(Ticket ticket) => db.Tickets.Remove(ticket);
}

public class OrderRepository(AppDbContext db) : IOrderRepository
{
    public Task<Order?> FindByIdAsync(Guid id, CancellationToken ct = default) =>
        db.Orders.FirstOrDefaultAsync(o => o.Id == id, ct);

    public Task<Order?> FindWithItemsAsync(Guid id, CancellationToken ct = default) =>
        db.Orders.Include(o => o.Items).ThenInclude(i => i.Ticket)
            .FirstOrDefaultAsync(o => o.Id == id, ct);

    public async Task<IReadOnlyList<Order>> ListForUserAsync(
        Guid userId, CancellationToken ct = default) =>
        await db.Orders.AsNoTracking()
            .Include(o => o.Items).ThenInclude(i => i.Ticket)
            .Where(o => o.UserId == userId)
            .OrderByDescending(o => o.CreatedAt)
            .Take(100).ToListAsync(ct);

    public async Task<IReadOnlyList<Order>> ListAllAsync(CancellationToken ct = default) =>
        await db.Orders.AsNoTracking()
            .Include(o => o.User)
            .Include(o => o.Items).ThenInclude(i => i.Ticket)
            .OrderByDescending(o => o.CreatedAt)
            .Take(500).ToListAsync(ct);

    public async Task AddAsync(Order order, CancellationToken ct = default) =>
        await db.Orders.AddAsync(order, ct);

    public async Task<IReadOnlyList<Order>> FindPendingByTicketIdsAsync(
        IReadOnlyCollection<Guid> ticketIds, CancellationToken ct = default) =>
        await db.Orders.Include(o => o.Items)
            .Where(o => o.Status == OrderStatus.Pending &&
                        o.Items.Any(i => ticketIds.Contains(i.TicketId)))
            .ToListAsync(ct);

    public void RemoveItems(IEnumerable<OrderItem> items) =>
        db.OrderItems.RemoveRange(items);
}

public class PaymentRepository(AppDbContext db) : IPaymentRepository
{
    public Task<Payment?> FindByIdAsync(Guid id, CancellationToken ct = default) =>
        db.Payments.FirstOrDefaultAsync(p => p.Id == id, ct);

    public Task<Payment?> FindByProviderRefAsync(
        string provider, string providerRef, CancellationToken ct = default) =>
        db.Payments.FirstOrDefaultAsync(
            p => p.Provider == provider && p.ProviderRef == providerRef, ct);

    public async Task AddAsync(Payment payment, CancellationToken ct = default) =>
        await db.Payments.AddAsync(payment, ct);
}

public class DrawResultRepository(AppDbContext db) : IDrawResultRepository
{
    public Task<DrawResult?> FindAsync(DateOnly drawDate, CancellationToken ct = default) =>
        db.DrawResults.FirstOrDefaultAsync(r => r.DrawDate == drawDate, ct);

    public async Task AddAsync(DrawResult result, CancellationToken ct = default) =>
        await db.DrawResults.AddAsync(result, ct);
}

public class UnitOfWork(AppDbContext db) : IUnitOfWork
{
    public async Task SaveChangesAsync(CancellationToken ct = default)
    {
        try
        {
            await db.SaveChangesAsync(ct);
        }
        catch (DbUpdateConcurrencyException)
        {
            throw new DomainException("A ticket was just taken by someone else. Please retry.", 409);
        }
        catch (DbUpdateException ex) when (ex.InnerException?.Message.Contains("UQ_") == true
            || ex.InnerException?.Message.Contains("IX_") == true
            || ex.InnerException?.Message.Contains("unique", StringComparison.OrdinalIgnoreCase) == true)
        {
            throw new DomainException("A conflicting record already exists.", 409);
        }
    }
}
