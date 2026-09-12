using Solution.BuildingBlocks.Core.Exceptions;
using Solution.UniLeague.API.Dtos;
using Solution.UniLeague.API.Public;
using Solution.UniLeague.Core.Domain;
using Solution.UniLeague.Core.Domain.RepositoryInterfaces;
using Solution.UniLeague.Core.RepositoryInterfaces;

namespace Solution.UniLeague.Core.UseCases;

public class AdminMatchService : IAdminMatchService
{
    private readonly IMatchRepository _matchRepository;
    private readonly ILeagueRepository _leagueRepository;
    private readonly ITeamRepository _teamRepository;
    private readonly IStandingsRecalculationService _standingsRecalculation;
    private readonly IMatchResultBuilder _matchResultBuilder;

    public AdminMatchService(
        IMatchRepository matchRepository,
        ILeagueRepository leagueRepository,
        ITeamRepository teamRepository,
        IStandingsRecalculationService standingsRecalculation,
        IMatchResultBuilder matchResultBuilder)
    {
        _matchRepository = matchRepository;
        _leagueRepository = leagueRepository;
        _teamRepository = teamRepository;
        _standingsRecalculation = standingsRecalculation;
        _matchResultBuilder = matchResultBuilder;
    }

    public AdminMatchDto ScheduleMatch(long leagueId, ScheduleMatchDto request)
    {
        var league = _leagueRepository.GetByIdWithStandings(leagueId)
            ?? throw new NotFoundException($"League with id {leagueId} was not found.");

        EnsureTeamsInLeague(league, request.HomeTeamId, request.AwayTeamId);

        var homeTeam = _teamRepository.GetByIdWithPlayers(request.HomeTeamId);
        var awayTeam = _teamRepository.GetByIdWithPlayers(request.AwayTeamId);

        var match = new Match(
            leagueId,
            request.RoundNumber,
            homeTeam.Id, homeTeam.Name, homeTeam.LogoUrl ?? string.Empty,
            awayTeam.Id, awayTeam.Name, awayTeam.LogoUrl ?? string.Empty,
            request.ScheduledAt);

        _matchRepository.Add(match);

        return ToDto(match);
    }

    public void UpdateMatch(long matchId, UpdateMatchDto request)
    {
        var match = _matchRepository.GetByIdWithResult(matchId)
            ?? throw new NotFoundException($"Match with id {matchId} was not found.");

        if (request.ScheduledAt is { } scheduledAt)
            match.Reschedule(scheduledAt);

        if (request.HomeTeamId is { } homeTeamId && request.AwayTeamId is { } awayTeamId)
        {
            var league = _leagueRepository.GetByIdWithStandings(match.LeagueId)
                ?? throw new NotFoundException($"League with id {match.LeagueId} was not found.");

            EnsureTeamsInLeague(league, homeTeamId, awayTeamId);

            var homeTeam = _teamRepository.GetByIdWithPlayers(homeTeamId);
            var awayTeam = _teamRepository.GetByIdWithPlayers(awayTeamId);

            match.SetTeams(
                homeTeam.Id, homeTeam.Name, homeTeam.LogoUrl ?? string.Empty,
                awayTeam.Id, awayTeam.Name, awayTeam.LogoUrl ?? string.Empty);
        }

        _matchRepository.Save(match);
    }

    public void DeleteMatch(long matchId)
    {
        var match = _matchRepository.GetByIdWithResult(matchId)
            ?? throw new NotFoundException($"Match with id {matchId} was not found.");

        if (match.HasResult)
            throw new ArgumentException("Cannot delete a match that already has a result.");

        _matchRepository.Delete(match);
    }

    public void UpdateResult(long matchId, SubmitMatchResultDto request)
    {
        var match = _matchRepository.GetByIdWithResult(matchId)
            ?? throw new NotFoundException($"Match with id {matchId} was not found.");

        var league = _leagueRepository.GetByIdWithStandings(match.LeagueId)
            ?? throw new NotFoundException($"League with id {match.LeagueId} was not found.");

        if (match.Stage == MatchStage.RegularSeason)
            HandlePlayoffDesync(match.LeagueId);

        var result = _matchResultBuilder.Build(league.Sport, match, request);

        match.SetResult(result);
        _matchRepository.Save(match);

        _standingsRecalculation.Recalculate(match.LeagueId);
    }

    public void ClearResult(long matchId)
    {
        var match = _matchRepository.GetByIdWithResult(matchId)
            ?? throw new NotFoundException($"Match with id {matchId} was not found.");

        if (match.Stage == MatchStage.RegularSeason)
            HandlePlayoffDesync(match.LeagueId);

        match.ClearResult();
        _matchRepository.Save(match);

        _standingsRecalculation.Recalculate(match.LeagueId);
    }

    // Plej-of se generise iz regularne tabele - ako se ta tabela menja posle generisanja,
    // vec generisan plej-of vise ne odgovara ispravljenom plasmanu
    private void HandlePlayoffDesync(long leagueId)
    {
        var playoffMatches = _matchRepository.GetPlayoffMatchesByLeague(leagueId);
        if (playoffMatches.Count == 0)
            return;

        if (playoffMatches.Any(m => m.HasResult))
            throw new ArgumentException(
                "Cannot change a regular season result while playoff matches already have recorded results.");

        // Nijedan plej-of mec jos nema rezultat - brisemo ih, PlayoffService ce ih
        // ponovo generisati iz ispravljene tabele pri sledecem citanju
        _matchRepository.DeleteRange(playoffMatches);
    }

    private static void EnsureTeamsInLeague(League league, long homeTeamId, long awayTeamId)
    {
        var teamIds = league.Standings.Select(s => (long)s.TeamId).ToHashSet();

        if (!teamIds.Contains(homeTeamId))
            throw new ArgumentException($"Team {homeTeamId} is not registered in league {league.Id}.");
        if (!teamIds.Contains(awayTeamId))
            throw new ArgumentException($"Team {awayTeamId} is not registered in league {league.Id}.");
    }

    private static AdminMatchDto ToDto(Match match) => new()
    {
        Id = match.Id,
        LeagueId = match.LeagueId,
        RoundNumber = match.RoundNumber,
        Stage = match.Stage.ToString(),
        HomeTeamId = match.HomeTeamId,
        HomeTeamName = match.HomeTeamName,
        AwayTeamId = match.AwayTeamId,
        AwayTeamName = match.AwayTeamName,
        ScheduledAt = match.ScheduledAt,
        HasResult = match.HasResult,
        Result = match.Result?.ToString()
    };
}
