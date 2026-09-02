using HtawPyi.Domain;
using Microsoft.EntityFrameworkCore;

namespace HtawPyi.Infrastructure;

/// <summary>Maps to the schema created by db/init/02-schema.sql.</summary>
public class AppDbContext(DbContextOptions<AppDbContext> options) : DbContext(options)
{
    public DbSet<User> Users => Set<User>();
    public DbSet<RefreshToken> RefreshTokens => Set<RefreshToken>();
    public DbSet<Ticket> Tickets => Set<Ticket>();
    public DbSet<Order> Orders => Set<Order>();
    public DbSet<OrderItem> OrderItems => Set<OrderItem>();
    public DbSet<Payment> Payments => Set<Payment>();
    public DbSet<DrawResult> DrawResults => Set<DrawResult>();

    protected override void OnModelCreating(ModelBuilder b)
    {
        b.Entity<User>(e =>
        {
            e.ToTable("Users");
            e.Property(x => x.Email).HasMaxLength(256);
            e.HasIndex(x => x.Email).IsUnique();
            e.Property(x => x.PasswordHash).HasMaxLength(500);
            e.Property(x => x.Role).HasConversion<string>().HasMaxLength(20);
        });

        b.Entity<RefreshToken>(e =>
        {
            e.ToTable("RefreshTokens");
            e.Property(x => x.TokenHash).HasMaxLength(500);
            e.HasOne(x => x.User).WithMany().HasForeignKey(x => x.UserId);
            e.HasIndex(x => x.TokenHash);
        });

        // SQL Server generates rowversion; SQLite (unit tests) cannot, so
        // there the column is a plain blob without concurrency semantics.
        var isSqlServer = Database.ProviderName?.EndsWith("SqlServer") ?? false;

        b.Entity<Ticket>(e =>
        {
            e.ToTable("Tickets");
            e.Property(x => x.Number).HasMaxLength(6).IsFixedLength();
            e.Property(x => x.Price).HasPrecision(10, 2);
            e.Property(x => x.Status).HasConversion<string>().HasMaxLength(20);
            if (isSqlServer)
                e.Property(x => x.RowVersion).IsRowVersion();
            else
                e.Property(x => x.RowVersion)
                    .HasDefaultValue(new byte[] { 0 })
                    .ValueGeneratedNever();
            e.HasIndex(x => new { x.DrawDate, x.Number }).IsUnique();
        });

        b.Entity<Order>(e =>
        {
            e.ToTable("Orders");
            e.Property(x => x.Status).HasConversion<string>().HasMaxLength(20);
            e.Property(x => x.Total).HasPrecision(12, 2);
            e.HasOne(x => x.User).WithMany().HasForeignKey(x => x.UserId);
            e.HasMany(x => x.Items).WithOne(x => x.Order).HasForeignKey(x => x.OrderId);
        });

        b.Entity<OrderItem>(e =>
        {
            e.ToTable("OrderItems");
            e.HasKey(x => new { x.OrderId, x.TicketId });
            e.HasIndex(x => x.TicketId).IsUnique();
            e.Property(x => x.PriceAtPurchase).HasPrecision(10, 2);
            e.HasOne(x => x.Ticket).WithMany().HasForeignKey(x => x.TicketId);
        });

        b.Entity<Payment>(e =>
        {
            e.ToTable("Payments");
            e.Property(x => x.Provider).HasMaxLength(20);
            e.Property(x => x.ProviderRef).HasMaxLength(100);
            e.Property(x => x.Amount).HasPrecision(12, 2);
            e.Property(x => x.Status).HasConversion<string>().HasMaxLength(20);
            e.HasOne(x => x.Order).WithMany().HasForeignKey(x => x.OrderId);
            e.HasIndex(x => new { x.Provider, x.ProviderRef })
                .IsUnique()
                .HasFilter("[ProviderRef] IS NOT NULL");
        });

        b.Entity<DrawResult>(e =>
        {
            e.ToTable("DrawResults");
            e.HasKey(x => x.DrawDate);
        });
    }
}
