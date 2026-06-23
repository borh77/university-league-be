using Solution.UniLeague.Core.Domain;
using Solution.UniLeague.Core.Domain.RepositoryInterfaces;
using Solution.UniLeague.Core.RepositoryInterfaces;

namespace Solution.UniLeague.Core.UseCases;

public class PlayoffService : IPlayoffService
{
    private readonly ILeagueRepository _leagueRepository;
    private readonly IMatchRepository _matchRepository;

    public PlayoffService(ILeagueRepository leagueRepository, IMatchRepository matchRepository)
    {
        _leagueRepository = leagueRepository;
        _matchRepository = matchRepository;
    }

    public void EnsurePlayoffsGenerated(long leagueId)
    {
        if (_matchRepository.HasPlayoffSemifinals(leagueId))
            return;

        var regularSeasonMatches = _matchRepository.GetRegularSeasonMatchesByLeague(leagueId);
        if (regularSeasonMatches.Count == 0 || regularSeasonMatches.Any(m => !m.HasResult))
            return;

        var league = _leagueRepository.GetByIdWithStandings(leagueId);
        if (league is null || league.Standings.Count < 4)
            return;

        var sortedStandings = new StandingsSorter()
            .Sort(league.Sport, league.Standings, regularSeasonMatches)
            .Take(4)
            .ToList();

        if (sortedStandings.Count < 4)
            return;

        var roundNumber = regularSeasonMatches.Max(m => m.RoundNumber) + 1;
        var firstScheduledAt = regularSeasonMatches.Max(m => m.ScheduledAt).AddDays(7);

        var semifinals = new[]
        {
            CreateSemifinal(leagueId, roundNumber, sortedStandings[0], 1, sortedStandings[3], 4, firstScheduledAt),
            CreateSemifinal(leagueId, roundNumber, sortedStandings[1], 2, sortedStandings[2], 3, firstScheduledAt.AddHours(2))
        };

        _matchRepository.AddPlayoffSemifinalsIfNone(leagueId, semifinals);
    }

    private static Match CreateSemifinal(
        long leagueId,
        int roundNumber,
        StandingEntry home,
        int homeSeed,
        StandingEntry away,
        int awaySeed,
        DateTime scheduledAt)
    {
        return new Match(
            leagueId,
            roundNumber,
            home.TeamId,
            home.TeamName,
            home.LogoUrl ?? string.Empty,
            away.TeamId,
            away.TeamName,
            away.LogoUrl ?? string.Empty,
            scheduledAt,
            MatchStage.PlayoffSemifinal,
            homeSeed,
            awaySeed);
    }
}
