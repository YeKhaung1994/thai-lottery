using System.Globalization;
using HtawPyi.Domain;

namespace HtawPyi.Application;

public class AdminTicketService(
    ITicketRepository tickets,
    IOrderRepository orders,
    IUnitOfWork uow,
    TimeProvider clock)
{
    private static readonly TimeSpan BangkokOffset = TimeSpan.FromHours(7);

    /// <summary>
    /// The only draw tickets may be uploaded for: draws are on the 1st and
    /// 16th (Thailand time) — before the 16th the next draw is the 16th,
    /// otherwise the 1st of the following month.
    /// </summary>
    public static DateOnly NextDrawDate(DateTimeOffset utcNow)
    {
        var today = DateOnly.FromDateTime((utcNow + BangkokOffset).UtcDateTime);
        return today.Day < 16
            ? new DateOnly(today.Year, today.Month, 16)
            : new DateOnly(today.Year, today.Month, 1).AddMonths(1);
    }

    public async Task<UploadReport> UploadAsync(
        Guid adminId, IReadOnlyList<UploadTicketRequest> rows, CancellationToken ct = default)
    {
        var rejected = new List<UploadReportRow>();
        var toInsert = new List<Ticket>();
        var seenInBatch = new HashSet<(DateOnly, string)>();
        var now = clock.GetUtcNow().UtcDateTime;
        var nextDraw = NextDrawDate(clock.GetUtcNow());

        for (var i = 0; i < rows.Count; i++)
        {
            var row = rows[i];
            var rowNo = i + 1;
            if (!DateOnly.TryParseExact(row.DrawDate, "yyyy-MM-dd", CultureInfo.InvariantCulture,
                    DateTimeStyles.None, out var drawDate))
            {
                rejected.Add(new UploadReportRow(rowNo, row.Number, "Invalid draw date (yyyy-MM-dd)."));
                continue;
            }
            if (drawDate != nextDraw)
            {
                rejected.Add(new UploadReportRow(
                    rowNo, row.Number,
                    "Tickets can only be uploaded for the next draw " +
                    $"({nextDraw.ToString("yyyy-MM-dd", CultureInfo.InvariantCulture)})."));
                continue;
            }
            if (row.Number.Length != 6 || !row.Number.All(char.IsAsciiDigit))
            {
                rejected.Add(new UploadReportRow(rowNo, row.Number, "Number must be exactly 6 digits."));
                continue;
            }
            if (row.Price <= 0)
            {
                rejected.Add(new UploadReportRow(rowNo, row.Number, "Price must be positive."));
                continue;
            }
            if (!seenInBatch.Add((drawDate, row.Number)))
            {
                rejected.Add(new UploadReportRow(rowNo, row.Number, "Duplicate within this upload."));
                continue;
            }
            if (await tickets.ExistsAsync(drawDate, row.Number, ct))
            {
                rejected.Add(new UploadReportRow(rowNo, row.Number, "Already exists for this draw."));
                continue;
            }
            toInsert.Add(new Ticket
            {
                DrawDate = drawDate,
                Number = row.Number,
                Price = row.Price,
                UploadedBy = adminId,
                CreatedAt = now
            });
        }

        if (toInsert.Count > 0)
        {
            await tickets.AddRangeAsync(toInsert, ct);
            await uow.SaveChangesAsync(ct);
        }
        return new UploadReport(toInsert.Count, rejected);
    }

    public async Task<IReadOnlyList<AdminTicketDto>> ListAsync(
        DateOnly? drawDate, TicketStatus? status, CancellationToken ct = default)
    {
        var list = await tickets.ListAsync(drawDate, status, ct);
        return list.Select(t => new AdminTicketDto(
            t.Id,
            t.DrawDate.ToString("yyyy-MM-dd", CultureInfo.InvariantCulture),
            t.Number, t.Price, t.Status.ToString()))
            .ToList();
    }

    public async Task DeleteAsync(Guid ticketId, CancellationToken ct = default)
    {
        var ticket = await tickets.FindByIdAsync(ticketId, ct)
            ?? throw new DomainException("Ticket not found.", 404);
        if (ticket.Status != TicketStatus.Available)
            throw new DomainException("Only Available tickets can be deleted.", 409);
        tickets.Remove(ticket);
        await uow.SaveChangesAsync(ct);
    }

    public async Task<IReadOnlyList<AdminOrderDto>> ListOrdersAsync(CancellationToken ct = default)
    {
        var all = await orders.ListAllAsync(ct);
        return all.Select(o => new AdminOrderDto(
            o.Id,
            o.User?.Email ?? "?",
            o.Status.ToString(),
            o.Total,
            o.CreatedAt,
            o.Items.Select(i => i.Ticket?.Number ?? "?").ToList()))
            .ToList();
    }
}
