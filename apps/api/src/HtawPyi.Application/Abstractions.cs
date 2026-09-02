using HtawPyi.Domain;

namespace HtawPyi.Application;

// ------------------------------------------------------------ Repositories

public interface IUserRepository
{
    Task<User?> FindByEmailAsync(string email, CancellationToken ct = default);
    Task<User?> FindByIdAsync(Guid id, CancellationToken ct = default);
    Task AddAsync(User user, CancellationToken ct = default);
}

public interface IRefreshTokenRepository
{
    Task<RefreshToken?> FindByHashAsync(string tokenHash, CancellationToken ct = default);
    Task AddAsync(RefreshToken token, CancellationToken ct = default);
}

public interface ITicketRepository
{
    Task<IReadOnlyList<Ticket>> SearchAsync(
        DateOnly? drawDate, string? query, DateTime now, CancellationToken ct = default);
    Task<IReadOnlyList<Ticket>> FindByIdsAsync(
        IReadOnlyCollection<Guid> ids, CancellationToken ct = default);
    Task<IReadOnlyList<Ticket>> ListAsync(
        DateOnly? drawDate, TicketStatus? status, CancellationToken ct = default);
    Task<Ticket?> FindByIdAsync(Guid id, CancellationToken ct = default);
    Task AddRangeAsync(IEnumerable<Ticket> tickets, CancellationToken ct = default);
    Task<bool> ExistsAsync(DateOnly drawDate, string number, CancellationToken ct = default);
    void Remove(Ticket ticket);
}

public interface IOrderRepository
{
    Task<Order?> FindByIdAsync(Guid id, CancellationToken ct = default);
    Task<Order?> FindWithItemsAsync(Guid id, CancellationToken ct = default);
    Task<IReadOnlyList<Order>> ListForUserAsync(Guid userId, CancellationToken ct = default);
    Task<IReadOnlyList<Order>> ListAllAsync(CancellationToken ct = default);
    Task AddAsync(Order order, CancellationToken ct = default);
    /// <summary>Pending orders holding any of these tickets (with items).</summary>
    Task<IReadOnlyList<Order>> FindPendingByTicketIdsAsync(
        IReadOnlyCollection<Guid> ticketIds, CancellationToken ct = default);
    void RemoveItems(IEnumerable<OrderItem> items);
}

public interface IPaymentRepository
{
    Task<Payment?> FindByIdAsync(Guid id, CancellationToken ct = default);
    Task<Payment?> FindByProviderRefAsync(
        string provider, string providerRef, CancellationToken ct = default);
    Task AddAsync(Payment payment, CancellationToken ct = default);
}

public interface IDrawResultRepository
{
    Task<DrawResult?> FindAsync(DateOnly drawDate, CancellationToken ct = default);
    Task AddAsync(DrawResult result, CancellationToken ct = default);
}

/// <summary>One SaveChanges per use-case; repositories only stage changes.</summary>
public interface IUnitOfWork
{
    Task SaveChangesAsync(CancellationToken ct = default);
}

// ----------------------------------------------------------- Infrastructure

public interface IPasswordHasher
{
    string Hash(string password);
    bool Verify(string password, string hash);
}

public interface ITokenService
{
    string CreateAccessToken(User user);
    (string Token, string Hash, DateTime ExpiresAt) CreateRefreshToken();
    string HashRefreshToken(string token);
}

/// <summary>Fetches official draw results from GLO, server-to-server.</summary>
public interface IGloClient
{
    Task<string?> FetchResultJsonAsync(DateOnly drawDate, CancellationToken ct = default);
}

public record PaymentInitiation(string RedirectUrl, string? ProviderRef);

public interface IPaymentProvider
{
    string Name { get; }
    Task<PaymentInitiation> InitiateAsync(Payment payment, Order order, CancellationToken ct = default);
    /// <summary>Verify a provider callback; returns the ProviderRef and success flag, or null when the signature is invalid.</summary>
    (string ProviderRef, bool Succeeded, decimal Amount)? VerifyCallback(string rawBody, IDictionary<string, string> headers);
}

public class DomainException(string message, int statusCode = 400) : Exception(message)
{
    public int StatusCode { get; } = statusCode;
}
