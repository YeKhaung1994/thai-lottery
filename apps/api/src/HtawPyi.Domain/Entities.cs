namespace HtawPyi.Domain;

public enum UserRole { Customer, Admin }

public enum TicketStatus { Available, Reserved, Sold }

public enum OrderStatus { Pending, Paid, Failed, Expired }

public enum PaymentStatus { Initiated, Succeeded, Failed }

public class User
{
    public Guid Id { get; set; }
    public required string Email { get; set; }
    public required string PasswordHash { get; set; }
    public UserRole Role { get; set; } = UserRole.Customer;
    public DateTime CreatedAt { get; set; }
}

public class RefreshToken
{
    public Guid Id { get; set; }
    public Guid UserId { get; set; }
    public required string TokenHash { get; set; }
    public DateTime ExpiresAt { get; set; }
    public DateTime? RevokedAt { get; set; }
    public DateTime CreatedAt { get; set; }
    public User? User { get; set; }

    public bool IsActive(DateTime now) => RevokedAt is null && ExpiresAt > now;
}

public class Ticket
{
    public Guid Id { get; set; }
    public DateOnly DrawDate { get; set; }
    public required string Number { get; set; }
    public decimal Price { get; set; }
    public TicketStatus Status { get; set; } = TicketStatus.Available;
    public DateTime? ReservedUntil { get; set; }
    public Guid UploadedBy { get; set; }
    public byte[] RowVersion { get; set; } = [];
    public DateTime CreatedAt { get; set; }

    /// <summary>A ticket can be bought if Available, or Reserved past its hold.</summary>
    public bool IsPurchasable(DateTime now) =>
        Status == TicketStatus.Available ||
        (Status == TicketStatus.Reserved && ReservedUntil is not null && ReservedUntil < now);
}

public class Order
{
    public Guid Id { get; set; }
    public Guid UserId { get; set; }
    public OrderStatus Status { get; set; } = OrderStatus.Pending;
    public decimal Total { get; set; }
    public DateTime CreatedAt { get; set; }
    public User? User { get; set; }
    public List<OrderItem> Items { get; set; } = [];
}

public class OrderItem
{
    public Guid OrderId { get; set; }
    public Guid TicketId { get; set; }
    public decimal PriceAtPurchase { get; set; }
    public Order? Order { get; set; }
    public Ticket? Ticket { get; set; }
}

public class Payment
{
    public Guid Id { get; set; }
    public Guid OrderId { get; set; }
    public required string Provider { get; set; }
    public string? ProviderRef { get; set; }
    public decimal Amount { get; set; }
    public PaymentStatus Status { get; set; } = PaymentStatus.Initiated;
    public string? RawCallback { get; set; }
    public DateTime CreatedAt { get; set; }
    public Order? Order { get; set; }
}

public class DrawResult
{
    public DateOnly DrawDate { get; set; }
    public DateTime FetchedAt { get; set; }
    public required string ResultJson { get; set; }
}
