using AutoMapper;
using Solution.BuildingBlocks.Core.Exceptions;
using Solution.UniLeague.API.Dtos;
using Solution.UniLeague.API.Public;
using Solution.UniLeague.Core.Domain;
using Solution.UniLeague.Core.Domain.RepositoryInterfaces;
using Solution.UniLeague.Core.RepositoryInterfaces;

namespace Solution.UniLeague.Core.UseCases;

public class DelegateMatchService : IDelegateMatchService
{
    private readonly IMatchRepository _matchRepository;
    private readonly ILeagueRepository _leagueRepository;
    private readonly ITeamRepository _teamRepository;
    private readonly IStandingsRecalculationService _standingsRecalculation;
    private readonly IMapper _mapper;

    public DelegateMatchService(
        IMatchRepository matchRepository,
        ILeagueRepository leagueRepository,
        ITeamRepository teamRepository,
        IStandingsRecalculationService standingsRecalculation,
        IMapper mapper)
    {
        _matchRepository = matchRepository;
        _leagueRepository = leagueRepository;
        _teamRepository = teamRepository;
        _standingsRecalculation = standingsRecalculation;
        _mapper = mapper;
    }

    public DelegateMatchDto GetMatchForEntry(long matchId)
    {
        var match = _matchRepository.GetByIdWithResult(matchId)
            ?? throw new NotFoundException($"Match with id {matchId} was not found.");

        var league = _leagueRepository.GetByIdWithStandings(match.LeagueId)
            ?? throw new NotFoundException($"League with id {match.LeagueId} was not found.");

        var homeTeam = _teamRepository.GetByIdWithPlayers(match.HomeTeamId);
        var awayTeam = _teamRepository.GetByIdWithPlayers(match.AwayTeamId);

        var dto = new DelegateMatchDto
        {
            Id = match.Id,
            LeagueId = match.LeagueId,
            Sport = league.Sport.ToString(),
            RoundNumber = match.RoundNumber,
            ScheduledAt = match.ScheduledAt,
            Stage = match.Stage.ToString(),
            IsPlayoff = match.IsPlayoff,
            HomeTeamId = match.HomeTeamId,
            HomeTeamName = match.HomeTeamName,
            AwayTeamId = match.AwayTeamId,
            AwayTeamName = match.AwayTeamName,
            HasResult = match.HasResult,
            Result = match.Result?.ToString(),
            HomeRoster = MapRoster(homeTeam),
            AwayRoster = MapRoster(awayTeam)
        };

        if (match.Result is { } result)
        {
            if (result.HasQuarters)
                dto.Quarters = _mapper.Map<List<QuarterScoreDto>>(result.Quarters);
            if (result.HasSets)
                dto.Sets = _mapper.Map<List<SetScoreDto>>(result.Sets);
            if (result.HasGoals)
                dto.Goals = _mapper.Map<List<GoalEventDto>>(result.Goals);
            if (result.HasPlayerStats)
                dto.PlayerStats = _mapper.Map<List<PlayerStatLineDto>>(result.PlayerStats);
        }

        return dto;
    }

    public void SubmitResult(long matchId, SubmitMatchResultDto request)
    {
        var match = _matchRepository.GetByIdWithResult(matchId)
            ?? throw new NotFoundException($"Match with id {matchId} was not found.");

        var league = _leagueRepository.GetByIdWithStandings(match.LeagueId)
            ?? throw new NotFoundException($"League with id {match.LeagueId} was not found.");

        var result = BuildResult(league.Sport, match, request);

        match.SetResult(result);
        _matchRepository.SaveResult(match);

        _standingsRecalculation.Recalculate(match.LeagueId);
    }

    private MatchResult BuildResult(Sport sport, Match match, SubmitMatchResultDto request)
    {
        return sport switch
        {
            Sport.Basketball => MatchResult.CreateWithPlayerStats(
                request.HomeScore, request.AwayScore,
                ResolvePlayerStats(match, RequirePlayerStats(request, "basketball")),
                quarters: RequireQuarters(request)),

            Sport.Volleyball => MatchResult.CreateWithPlayerStats(
                request.HomeScore, request.AwayScore,
                ResolvePlayerStats(match, RequirePlayerStats(request, "volleyball")),
                sets: RequireSets(request)),

            _ => request.Goals is { Count: > 0 }
                ? MatchResult.CreateWithGoals(
                    request.HomeScore, request.AwayScore,
                    request.Goals.Select(g => GoalEvent.Create(
                        g.ScorerName, g.TeamName, g.IsHomeTeamGoal, g.Minute)))
                : MatchResult.Create(request.HomeScore, request.AwayScore)
        };
    }

    private static List<PlayerStatInputDto> RequirePlayerStats(SubmitMatchResultDto request, string sport)
    {
        if (request.PlayerStats is not { Count: > 0 })
            throw new ArgumentException($"Player stats are required for {sport} results.");
        return request.PlayerStats;
    }

    private static List<QuarterScore> RequireQuarters(SubmitMatchResultDto request)
    {
        if (request.Quarters is not { Count: > 0 })
            throw new ArgumentException("Quarters are required for basketball results.");
        return request.Quarters
            .Select(q => QuarterScore.Create(q.QuarterNumber, q.HomeScore, q.AwayScore))
            .ToList();
    }

    private static List<SetScore> RequireSets(SubmitMatchResultDto request)
    {
        if (request.Sets is not { Count: > 0 })
            throw new ArgumentException("Sets are required for volleyball results.");
        return request.Sets
            .Select(s => SetScore.Create(s.SetNumber, s.HomeScore, s.AwayScore))
            .ToList();
    }

    private List<PlayerStatLine> ResolvePlayerStats(Match match, List<PlayerStatInputDto> inputs)
    {
        var homePlayers = _teamRepository.GetByIdWithPlayers(match.HomeTeamId).Players;
        var awayPlayers = _teamRepository.GetByIdWithPlayers(match.AwayTeamId).Players;

        return inputs.Select(input =>
        {
            var roster = input.IsHomeTeam ? homePlayers : awayPlayers;
            var player = roster.FirstOrDefault(p => p.Id == input.PlayerId)
                ?? throw new ArgumentException(
                    $"Player {input.PlayerId} is not in the {(input.IsHomeTeam ? "home" : "away")} team roster.");

            return PlayerStatLine.Create(
                player.Id,
                $"{player.FirstName} {player.LastName}",
                player.JerseyNumber,
                input.IsHomeTeam,
                input.Points);
        }).ToList();
    }

    private List<PlayerDto> MapRoster(Team team)
        => _mapper.Map<List<PlayerDto>>(team.Players.OrderBy(p => p.JerseyNumber).ToList());
}
