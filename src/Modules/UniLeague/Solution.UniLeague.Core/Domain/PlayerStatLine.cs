using Solution.BuildingBlocks.Core.Domain;

namespace Solution.UniLeague.Core.Domain;

public class PlayerStatLine : ValueObject
{
    public long PlayerId { get; }
    public string PlayerName { get; }
    public int JerseyNumber { get; }
    public bool IsHomeTeam { get; }
    public int Points { get; }

    private PlayerStatLine() { } // EF

    private PlayerStatLine(long playerId, string playerName, int jerseyNumber, bool isHomeTeam, int points)
    {
        if (playerId == 0)
            throw new ArgumentException("Invalid PlayerId.", nameof(playerId));
        if (string.IsNullOrWhiteSpace(playerName))
            throw new ArgumentException("PlayerName is required.", nameof(playerName));
        if (jerseyNumber < 1 || jerseyNumber > 99)
            throw new ArgumentException("JerseyNumber must be between 1 and 99.", nameof(jerseyNumber));
        if (points < 0)
            throw new ArgumentException("Points cannot be negative.", nameof(points));

        PlayerId = playerId;
        PlayerName = playerName.Trim();
        JerseyNumber = jerseyNumber;
        IsHomeTeam = isHomeTeam;
        Points = points;
    }

    public static PlayerStatLine Create(long playerId, string playerName, int jerseyNumber, bool isHomeTeam, int points)
        => new(playerId, playerName, jerseyNumber, isHomeTeam, points);

    public override string ToString() => $"#{JerseyNumber} {PlayerName}: {Points}";

    protected override IEnumerable<object> GetEqualityComponents()
    {
        yield return PlayerId;
        yield return PlayerName;
        yield return JerseyNumber;
        yield return IsHomeTeam;
        yield return Points;
    }
}
