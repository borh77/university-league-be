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

    // Parameterless constructor for EF Core
    private Match() { }

    public Match(
        long leagueId,
        int roundNumber,
        long homeTeamId, string homeTeamName, string homeTeamLogoUrl,
        long awayTeamId, string awayTeamName, string awayTeamLogoUrl,
        DateTime scheduledAt)
    {
        if (leagueId <= 0) throw new ArgumentException("Invalid LeagueId.");
        if (roundNumber <= 0) throw new ArgumentException("RoundNumber must be positive.");
        if (string.IsNullOrWhiteSpace(homeTeamName)) throw new ArgumentException("HomeTeamName is required.");
        if (string.IsNullOrWhiteSpace(awayTeamName)) throw new ArgumentException("AwayTeamName is required.");
        if (homeTeamId == awayTeamId) throw new ArgumentException("Home and away team must be different.");

        LeagueId = leagueId;
        RoundNumber = roundNumber;
        HomeTeamId = homeTeamId;
        HomeTeamName = homeTeamName;
        HomeTeamLogoUrl = homeTeamLogoUrl;
        AwayTeamId = awayTeamId;
        AwayTeamName = awayTeamName;
        AwayTeamLogoUrl = awayTeamLogoUrl;
        ScheduledAt = scheduledAt;
    }
}