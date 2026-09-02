using System.Globalization;
using HtawPyi.Application;

namespace HtawPyi.Tests;

public class NextDrawDateTests
{
    // The host machine runs a Thai (Buddhist-calendar) culture, so all test
    // parsing must be invariant or "2026" means B.E. 2026 (= C.E. 1483).
    private static DateTimeOffset Utc(string s) =>
        DateTimeOffset.Parse(s, CultureInfo.InvariantCulture);

    private static DateOnly Day(string s) =>
        DateOnly.Parse(s, CultureInfo.InvariantCulture);

    [Theory]
    [InlineData("2026-09-02T00:00:00Z", "2026-09-16")] // early month → the 16th
    [InlineData("2026-09-16T00:00:00Z", "2026-10-01")] // on the 16th → 1st of next
    [InlineData("2026-09-30T00:00:00Z", "2026-10-01")] // late month → 1st of next
    [InlineData("2026-12-20T00:00:00Z", "2027-01-01")] // year rollover
    public void ComputesTheOnlyUploadableDraw(string utcNow, string expected)
    {
        Assert.Equal(Day(expected), AdminTicketService.NextDrawDate(Utc(utcNow)));
    }

    [Fact]
    public void BangkokOffsetFlipsTheDayNearMidnightUtc()
    {
        // 17:30 UTC on the 15th is already the 16th in Bangkok (+7):
        // the next draw is therefore the 1st of the following month.
        var result = AdminTicketService.NextDrawDate(Utc("2026-09-15T17:30:00Z"));
        Assert.Equal(new DateOnly(2026, 10, 1), result);
    }
}
