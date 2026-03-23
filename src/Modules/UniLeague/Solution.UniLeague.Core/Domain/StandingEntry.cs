using Solution.BuildingBlocks.Core.Domain;

namespace Solution.UniLeague.Core.Domain;

public class StandingEntry : Entity
{
    public long  LeagueId { get; private set; }
    public int TeamId { get; private set; }

    public string TeamName { get; private set; }
    public string? LogoUrl { get; private set; }
    public int Played { get; private set; }
    public int Won { get; private set; }
    public int Drawn { get; private set; }
    public int Lost { get; private set; }
    public int Points { get; private set; }
    public int Scored { get; private set; }
    public int Conceded { get; private set; }
    public int Difference => Scored - Conceded;

    // Volleyball-specific (null for other sports)
    public int? SetWon { get; private set; }
    public int? SetLost { get; private set; }
    public int? SetDifference => SetWon.HasValue && SetLost.HasValue
        ? SetWon.Value - SetLost.Value
        : null;

    private StandingEntry() { }

    public StandingEntry(
        long leagueId,
        int teamId,
        string teamName,
        string? logoUrl,
        int played,
        int won,
        int drawn,
        int lost,
        int points,
        int scored,
        int conceded,
        int? setWon = null,
        int? setLost = null)
    {
        if (string.IsNullOrWhiteSpace(teamName))
            throw new ArgumentException("TeamName cannot be empty.");
        if (played < 0 || won < 0 || drawn < 0 || lost < 0 || points < 0 || scored < 0 || conceded < 0)
            throw new ArgumentException("Statistical values cannot be negative.");

        LeagueId = leagueId;
        TeamId = teamId;
        TeamName = teamName;
        LogoUrl = logoUrl;
        Played = played;
        Won = won;
        Drawn = drawn;
        Lost = lost;
        Points = points;
        Scored = scored;
        Conceded = conceded;
        SetWon = setWon;
        SetLost = setLost;
    }
}