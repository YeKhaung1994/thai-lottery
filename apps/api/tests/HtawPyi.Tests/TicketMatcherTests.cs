using System.Text.Json;
using HtawPyi.Domain;

namespace HtawPyi.Tests;

/// <summary>
/// Mirrors apps/customer/tests/unit/lotteryApi.spec.js against the same
/// real GLO fixture (draw of 2026-09-01, first prize 417212).
/// </summary>
public class TicketMatcherTests
{
    private static readonly string ResultJson = LoadFixture();

    private static string LoadFixture()
    {
        var raw = File.ReadAllText(
            Path.Combine(AppContext.BaseDirectory, "glo-latest.json"));
        using var doc = JsonDocument.Parse(raw);
        return doc.RootElement.GetProperty("response").GetProperty("data").GetRawText();
    }

    [Fact]
    public void FirstPrize_Wins()
    {
        var wins = TicketMatcher.CheckTicket(ResultJson, "417212");
        var win = Assert.Single(wins);
        Assert.Equal("1st Prize", win.Name);
        Assert.Equal(6_000_000m, win.Reward);
    }

    [Fact]
    public void AdjacentToFirst_Wins()
    {
        var win = Assert.Single(TicketMatcher.CheckTicket(ResultJson, "417211"));
        Assert.Equal("Adjacent to 1st", win.Name);
        Assert.Equal(100_000m, win.Reward);
    }

    [Fact]
    public void FrontAndBackThree_StackOnOneTicket()
    {
        var wins = TicketMatcher.CheckTicket(ResultJson, "257136");
        Assert.Equal(2, wins.Count);
        Assert.Equal(["3-Digit Front", "3-Digit Back"], wins.Select(w => w.Name));
        Assert.All(wins, w => Assert.Equal(4_000m, w.Reward));
    }

    [Fact]
    public void TwoDigit_Wins()
    {
        var win = Assert.Single(TicketMatcher.CheckTicket(ResultJson, "999904"));
        Assert.Equal("2-Digit", win.Name);
        Assert.Equal(2_000m, win.Reward);
    }

    [Fact]
    public void LosingTicket_ReturnsEmpty() =>
        Assert.Empty(TicketMatcher.CheckTicket(ResultJson, "123456"));

    [Theory]
    [InlineData("12345")]
    [InlineData("1234567")]
    [InlineData("12345a")]
    [InlineData("")]
    public void MalformedTicket_Throws(string ticket) =>
        Assert.Throws<ArgumentException>(() => TicketMatcher.CheckTicket(ResultJson, ticket));

    [Fact]
    public void MissingGroups_DoNotCrash()
    {
        var wins = TicketMatcher.CheckTicket(
            """{"first":{"price":"100.00","number":[{"round":1,"value":"123456"}]}}""",
            "123456");
        var win = Assert.Single(wins);
        Assert.Equal("1st Prize", win.Name);
    }
}
