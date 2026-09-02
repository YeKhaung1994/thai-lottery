using HtawPyi.Application;
using HtawPyi.Domain;
using HtawPyi.Infrastructure;
using Microsoft.Data.Sqlite;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Time.Testing;

namespace HtawPyi.Tests;

public class OrderServiceTests : IDisposable
{
    private readonly SqliteConnection _connection;
    private readonly AppDbContext _db;
    private readonly FakeTimeProvider _clock = new(
        new DateTimeOffset(2026, 9, 2, 10, 0, 0, TimeSpan.Zero));
    private readonly OrderService _service;
    private readonly Guid _userId;
    private readonly Guid _ticketId;

    private sealed class NullGlo : IGloClient
    {
        public Task<string?> FetchResultJsonAsync(DateOnly d, CancellationToken ct = default) =>
            Task.FromResult<string?>(null);
    }

    public OrderServiceTests()
    {
        _connection = new SqliteConnection("DataSource=:memory:");
        _connection.Open();
        _db = new AppDbContext(new DbContextOptionsBuilder<AppDbContext>()
            .UseSqlite(_connection).Options);
        _db.Database.EnsureCreated();

        var user = new User { Email = "buyer@test", PasswordHash = "x" };
        var admin = new User { Email = "admin@test", PasswordHash = "x", Role = UserRole.Admin };
        var ticket = new Ticket
        {
            DrawDate = new DateOnly(2026, 9, 16),
            Number = "123456",
            Price = 120m,
            UploadedBy = admin.Id,
            RowVersion = [0]
        };
        _db.AddRange(user, admin, ticket);
        _db.SaveChanges();
        _userId = user.Id;
        _ticketId = ticket.Id;

        _service = new OrderService(
            new TicketRepository(_db),
            new OrderRepository(_db),
            new PaymentRepository(_db),
            new DrawResultRepository(_db),
            new NullGlo(),
            new MockPaymentProvider(new PaymentOptions()),
            new UnitOfWork(_db),
            _clock);
    }

    [Fact]
    public async Task Create_ReservesTicketsForFifteenMinutes()
    {
        var response = await _service.CreateAsync(
            _userId, new CreateOrderRequest([_ticketId]));

        Assert.Equal(120m, response.Total);
        Assert.Equal("Mock", response.Provider);
        Assert.Contains("/pay/mock/", response.RedirectUrl);

        var ticket = await _db.Tickets.SingleAsync(t => t.Id == _ticketId);
        Assert.Equal(TicketStatus.Reserved, ticket.Status);
        Assert.Equal(_clock.GetUtcNow().UtcDateTime.AddMinutes(15), ticket.ReservedUntil);
    }

    [Fact]
    public async Task Create_RejectsAlreadyReservedTicket()
    {
        await _service.CreateAsync(_userId, new CreateOrderRequest([_ticketId]));
        var ex = await Assert.ThrowsAsync<DomainException>(() =>
            _service.CreateAsync(_userId, new CreateOrderRequest([_ticketId])));
        Assert.Equal(409, ex.StatusCode);
    }

    [Fact]
    public async Task Create_AllowsTicketWithExpiredReservation()
    {
        await _service.CreateAsync(_userId, new CreateOrderRequest([_ticketId]));
        _clock.Advance(TimeSpan.FromMinutes(16));
        var response = await _service.CreateAsync(_userId, new CreateOrderRequest([_ticketId]));
        Assert.Equal(120m, response.Total);
    }

    [Fact]
    public async Task Create_RejectsEmptyAndOversizedOrders()
    {
        await Assert.ThrowsAsync<DomainException>(() =>
            _service.CreateAsync(_userId, new CreateOrderRequest([])));
        await Assert.ThrowsAsync<DomainException>(() =>
            _service.CreateAsync(_userId, new CreateOrderRequest(
                Enumerable.Range(0, 11).Select(_ => Guid.NewGuid()).ToList())));
    }

    [Fact]
    public async Task SuccessfulPayment_MarksOrderPaidAndTicketsSold()
    {
        await _service.CreateAsync(_userId, new CreateOrderRequest([_ticketId]));
        var payment = await _db.Payments.SingleAsync();

        await _service.CompletePaymentAsync(payment.Id, succeeded: true, "raw");

        Assert.Equal(PaymentStatus.Succeeded, payment.Status);
        Assert.Equal(OrderStatus.Paid, (await _db.Orders.SingleAsync()).Status);
        var ticket = await _db.Tickets.SingleAsync(t => t.Id == _ticketId);
        Assert.Equal(TicketStatus.Sold, ticket.Status);
        Assert.Null(ticket.ReservedUntil);
    }

    [Fact]
    public async Task FailedPayment_ReleasesTickets()
    {
        await _service.CreateAsync(_userId, new CreateOrderRequest([_ticketId]));
        var payment = await _db.Payments.SingleAsync();

        await _service.CompletePaymentAsync(payment.Id, succeeded: false, null);

        Assert.Equal(OrderStatus.Failed, (await _db.Orders.SingleAsync()).Status);
        Assert.Equal(TicketStatus.Available,
            (await _db.Tickets.SingleAsync(t => t.Id == _ticketId)).Status);
    }

    [Fact]
    public async Task CompletePayment_IsIdempotent()
    {
        await _service.CreateAsync(_userId, new CreateOrderRequest([_ticketId]));
        var payment = await _db.Payments.SingleAsync();

        await _service.CompletePaymentAsync(payment.Id, succeeded: true, "first");
        await _service.CompletePaymentAsync(payment.Id, succeeded: false, "replay");

        Assert.Equal(PaymentStatus.Succeeded, payment.Status);
        Assert.Equal(OrderStatus.Paid, (await _db.Orders.SingleAsync()).Status);
        Assert.Equal("first", payment.RawCallback);
    }

    public void Dispose()
    {
        _db.Dispose();
        _connection.Dispose();
    }
}
