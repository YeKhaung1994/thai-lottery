using System.Text.Json;

namespace HtawPyi.Domain;

public record PrizeWin(string Name, decimal Reward);

/// <summary>
/// C# port of the customer app's checkTicket (apps/customer/src/services/
/// lotteryApi.js). Input is the GLO getLotteryResult "data" object stored
/// verbatim in DrawResults.ResultJson.
/// </summary>
public static class TicketMatcher
{
    private static readonly (string Key, string Name)[] PrizeGroups =
    [
        ("first", "1st Prize"),
        ("near1", "Adjacent to 1st"),
        ("second", "2nd Prize"),
        ("third", "3rd Prize"),
        ("fourth", "4th Prize"),
        ("fifth", "5th Prize")
    ];

    public static IReadOnlyList<PrizeWin> CheckTicket(string resultJson, string ticket)
    {
        if (ticket.Length != 6 || !ticket.All(char.IsAsciiDigit))
            throw new ArgumentException("Ticket must be exactly 6 digits.", nameof(ticket));

        using var doc = JsonDocument.Parse(resultJson);
        var data = doc.RootElement;
        var wins = new List<PrizeWin>();

        foreach (var (key, name) in PrizeGroups)
        {
            if (!data.TryGetProperty(key, out var group)) continue;
            if (Numbers(group).Contains(ticket))
                wins.Add(new PrizeWin(name, Reward(group)));
        }

        if (data.TryGetProperty("last3f", out var front3) &&
            Numbers(front3).Contains(ticket[..3]))
            wins.Add(new PrizeWin("3-Digit Front", Reward(front3)));

        if (data.TryGetProperty("last3b", out var back3) &&
            Numbers(back3).Contains(ticket[^3..]))
            wins.Add(new PrizeWin("3-Digit Back", Reward(back3)));

        if (data.TryGetProperty("last2", out var last2) &&
            Numbers(last2).Contains(ticket[^2..]))
            wins.Add(new PrizeWin("2-Digit", Reward(last2)));

        return wins;
    }

    private static IEnumerable<string> Numbers(JsonElement group)
    {
        if (!group.TryGetProperty("number", out var numbers)) yield break;
        foreach (var n in numbers.EnumerateArray())
        {
            if (n.TryGetProperty("value", out var value) && value.GetString() is { } s)
                yield return s;
        }
    }

    private static decimal Reward(JsonElement group) =>
        group.TryGetProperty("price", out var price) &&
        decimal.TryParse(price.GetString(), out var value)
            ? value
            : 0m;
}
