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
        EnsureSemifinalsGenerated(leagueId);
        EnsureFinalAndThirdPlaceGenerated(leagueId);
    }

    // Plej-of se generise iz regularne tabele, ali zreb je istorijska cinjenica u trenutku generisanja.
    // Ako plej-of mecevi jos nemaju rezultat - nisu se igrali, brisemo ih pa se ponovo generisu iz
    // ispravljene tabele. Ako vec imaju rezultat - vec se igraju, zreb ostaje netaknut, samo se
    // regularna tabela ispravlja (StandingsRecalculationService to radi posle ovog poziva).
    public void HandleRegularSeasonResultChanged(long leagueId)
    {
        var playoffMatches = _matchRepository.GetPlayoffMatchesByLeague(leagueId);
        if (playoffMatches.Count == 0)
            return;

        if (playoffMatches.Any(m => m.HasResult))
            return;

        _matchRepository.DeleteRange(playoffMatches);
    }

    private void EnsureSemifinalsGenerated(long leagueId)
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

    private void EnsureFinalAndThirdPlaceGenerated(long leagueId)
    {
        var playoffMatches = _matchRepository.GetPlayoffMatchesByLeague(leagueId);
        var hasFinal = playoffMatches.Any(m => m.Stage == MatchStage.PlayoffFinal);
        var hasThirdPlace = playoffMatches.Any(m => m.Stage == MatchStage.PlayoffThirdPlace);

        if (hasFinal && hasThirdPlace)
            return;

        var semifinals = playoffMatches
            .Where(m => m.Stage == MatchStage.PlayoffSemifinal)
            .OrderBy(m => m.HomeSeed)
            .ThenBy(m => m.ScheduledAt)
            .ToList();

        if (semifinals.Count != 2 || semifinals.Any(m => !m.HasResult))
            return;

        var firstWinner = semifinals[0].GetWinner();
        var firstLoser = semifinals[0].GetLoser();
        var secondWinner = semifinals[1].GetWinner();
        var secondLoser = semifinals[1].GetLoser();

        if (firstWinner is null || firstLoser is null || secondWinner is null || secondLoser is null)
            return;

        var roundNumber = semifinals.Max(m => m.RoundNumber) + 1;
        var finalScheduledAt = semifinals.Max(m => m.ScheduledAt).AddDays(7);
        var thirdPlaceScheduledAt = finalScheduledAt.AddHours(-2);

        var matchesToCreate = new List<Match>();

        if (!hasThirdPlace)
        {
            matchesToCreate.Add(CreatePlayoffMatch(
                leagueId,
                roundNumber,
                firstLoser,
                secondLoser,
                thirdPlaceScheduledAt,
                MatchStage.PlayoffThirdPlace));
        }

        if (!hasFinal)
        {
            matchesToCreate.Add(CreatePlayoffMatch(
                leagueId,
                roundNumber,
                firstWinner,
                secondWinner,
                finalScheduledAt,
                MatchStage.PlayoffFinal));
        }

        _matchRepository.AddPlayoffMatchesIfStagesMissing(
            leagueId,
            matchesToCreate.Select(m => m.Stage).ToList(),
            matchesToCreate);
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

    private static Match CreatePlayoffMatch(
        long leagueId,
        int roundNumber,
        MatchTeamSnapshot home,
        MatchTeamSnapshot away,
        DateTime scheduledAt,
        MatchStage stage)
    {
        return new Match(
            leagueId,
            roundNumber,
            home.TeamId,
            home.TeamName,
            home.TeamLogoUrl,
            away.TeamId,
            away.TeamName,
            away.TeamLogoUrl,
            scheduledAt,
            stage,
            home.Seed,
            away.Seed);
    }
}
