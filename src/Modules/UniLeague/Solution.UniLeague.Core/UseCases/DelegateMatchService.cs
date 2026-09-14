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
    private readonly IMatchResultBuilder _matchResultBuilder;
    private readonly IMapper _mapper;
    private readonly IPlayoffService _playoffService;

    public DelegateMatchService(
        IMatchRepository matchRepository,
        ILeagueRepository leagueRepository,
        ITeamRepository teamRepository,
        IStandingsRecalculationService standingsRecalculation,
        IMatchResultBuilder matchResultBuilder,
        IMapper mapper,
        IPlayoffService playoffService)
    {
        _matchRepository = matchRepository;
        _leagueRepository = leagueRepository;
        _teamRepository = teamRepository;
        _standingsRecalculation = standingsRecalculation;
        _matchResultBuilder = matchResultBuilder;
        _mapper = mapper;
        _playoffService = playoffService;
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

        var result = _matchResultBuilder.Build(league.Sport, match, request);

        if (match.Stage == MatchStage.RegularSeason)
            _playoffService.HandleRegularSeasonResultChanged(match.LeagueId);

        match.SetResult(result);
        _matchRepository.SaveResult(match);

        _standingsRecalculation.Recalculate(match.LeagueId);
    }

    private List<PlayerDto> MapRoster(Team team)
        => _mapper.Map<List<PlayerDto>>(team.Players.OrderBy(p => p.JerseyNumber).ToList());
}
