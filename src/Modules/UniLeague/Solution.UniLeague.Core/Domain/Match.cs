using Solution.BuildingBlocks.Core.Domain;

namespace Solution.UniLeague.Core.Domain;

public class Match : Entity
{
    public long LeagueId { get; init; }
    public int RoundNumber { get; init; }
    public long HomeTeamId { get; private set; }
    public string HomeTeamName { get; private set; }
    public string HomeTeamLogoUrl { get; private set; }
    public long AwayTeamId { get; private set; }
    public string AwayTeamName { get; private set; }
    public string AwayTeamLogoUrl { get; private set; }
    public DateTime ScheduledAt { get; private set; }
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
        if (leagueId == 0) throw new ArgumentException("Invalid LeagueId.");
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

    // Admin ponistava delegatov unos - koristi se i kad treba da se ispravi pogresan tim
    public void ClearResult()
    {
        Result = null;
    }

    // Termin se moze pomerati i za plej-of mecevi, nema dodatnog ogranicenja
    public void Reschedule(DateTime scheduledAt)
    {
        ScheduledAt = scheduledAt;
    }

    // Zamena timova je dozvoljena samo dok mec nema rezultat i nije plej-of
    // (plej-of timovi dolaze iz plasmana, ne biraju se rucno)
    public void SetTeams(
        long homeTeamId, string homeTeamName, string homeTeamLogoUrl,
        long awayTeamId, string awayTeamName, string awayTeamLogoUrl)
    {
        if (HasResult)
            throw new ArgumentException("Cannot change teams on a match that already has a result.");
        if (Stage != MatchStage.RegularSeason)
            throw new ArgumentException("Cannot change teams on a playoff match.");
        if (string.IsNullOrWhiteSpace(homeTeamName)) throw new ArgumentException("HomeTeamName is required.");
        if (string.IsNullOrWhiteSpace(awayTeamName)) throw new ArgumentException("AwayTeamName is required.");
        if (homeTeamId == awayTeamId) throw new ArgumentException("Home and away team must be different.");

        HomeTeamId = homeTeamId;
        HomeTeamName = homeTeamName;
        HomeTeamLogoUrl = homeTeamLogoUrl;
        AwayTeamId = awayTeamId;
        AwayTeamName = awayTeamName;
        AwayTeamLogoUrl = awayTeamLogoUrl;
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
