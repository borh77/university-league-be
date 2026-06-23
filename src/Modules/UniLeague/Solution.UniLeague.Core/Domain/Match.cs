using Solution.BuildingBlocks.Core.Domain;

namespace Solution.UniLeague.Core.Domain;

public class Match : Entity
{
    public long LeagueId { get; init; }
    public int RoundNumber { get; init; }
    public long HomeTeamId { get; init; }
    public string HomeTeamName { get; init; }
    public string HomeTeamLogoUrl { get; init; }
    public long AwayTeamId { get; init; }
    public string AwayTeamName { get; init; }
    public string AwayTeamLogoUrl { get; init; }
    public DateTime ScheduledAt { get; init; }
    public MatchStage Stage { get; private set; } = MatchStage.RegularSeason;
    public int? HomeSeed { get; private set; }
    public int? AwaySeed { get; private set; }
    public MatchResult? Result { get; private set; }

    // Parameterless constructor for EF Core
    private Match() { }

    public Match(
        long leagueId,
        int roundNumber,
        long homeTeamId, string homeTeamName, string homeTeamLogoUrl,
        long awayTeamId, string awayTeamName, string awayTeamLogoUrl,
        DateTime scheduledAt)
        : this(
            leagueId, roundNumber,
            homeTeamId, homeTeamName, homeTeamLogoUrl,
            awayTeamId, awayTeamName, awayTeamLogoUrl,
            scheduledAt,
            MatchStage.RegularSeason)
    {
    }

    public Match(
        long leagueId,
        int roundNumber,
        long homeTeamId, string homeTeamName, string homeTeamLogoUrl,
        long awayTeamId, string awayTeamName, string awayTeamLogoUrl,
        DateTime scheduledAt,
        MatchStage stage,
        int? homeSeed = null,
        int? awaySeed = null)
    {
        if (leagueId <= 0) throw new ArgumentException("Invalid LeagueId.");
        if (roundNumber <= 0) throw new ArgumentException("RoundNumber must be positive.");
        if (string.IsNullOrWhiteSpace(homeTeamName)) throw new ArgumentException("HomeTeamName is required.");
        if (string.IsNullOrWhiteSpace(awayTeamName)) throw new ArgumentException("AwayTeamName is required.");
        if (homeTeamId == awayTeamId) throw new ArgumentException("Home and away team must be different.");
        if (stage == MatchStage.RegularSeason && (homeSeed.HasValue || awaySeed.HasValue))
            throw new ArgumentException("Regular season matches cannot have playoff seeds.");
        if (stage != MatchStage.RegularSeason && (!homeSeed.HasValue || !awaySeed.HasValue))
            throw new ArgumentException("Playoff matches must have both seeds.");

        LeagueId = leagueId;
        RoundNumber = roundNumber;
        HomeTeamId = homeTeamId;
        HomeTeamName = homeTeamName;
        HomeTeamLogoUrl = homeTeamLogoUrl;
        AwayTeamId = awayTeamId;
        AwayTeamName = awayTeamName;
        AwayTeamLogoUrl = awayTeamLogoUrl;
        ScheduledAt = scheduledAt;
        Stage = stage;
        HomeSeed = homeSeed;
        AwaySeed = awaySeed;
    }

    public void SetResult(MatchResult result)
    {
        Result = result ?? throw new ArgumentNullException(nameof(result));
    }

    public bool HasResult => Result is not null;
    public bool IsPlayoff => Stage != MatchStage.RegularSeason;

    public MatchTeamSnapshot? GetWinner()
    {
        if (Result is null || Result.IsDraw())
            return null;

        return Result.HomeWon() ? GetHomeTeamSnapshot() : GetAwayTeamSnapshot();
    }

    public MatchTeamSnapshot? GetLoser()
    {
        if (Result is null || Result.IsDraw())
            return null;

        return Result.HomeWon() ? GetAwayTeamSnapshot() : GetHomeTeamSnapshot();
    }

    private MatchTeamSnapshot GetHomeTeamSnapshot()
        => new(HomeTeamId, HomeTeamName, HomeTeamLogoUrl, HomeSeed);

    private MatchTeamSnapshot GetAwayTeamSnapshot()
        => new(AwayTeamId, AwayTeamName, AwayTeamLogoUrl, AwaySeed);
}
