using AutoMapper;
using Solution.UniLeague.API.Dtos;
using Solution.UniLeague.API.Public;
using Solution.UniLeague.Core.Domain.RepositoryInterfaces;

namespace Solution.UniLeague.Core.UseCases;

public class LeagueService : ILeagueService
{
    private readonly IMatchRepository _matchRepository;
    private readonly IMapper _mapper;
    private readonly ITopScorerQueryService _topScorerQueryService;
    private readonly IPlayoffService _playoffService;

    public LeagueService(
        IMatchRepository matchRepository,
        IMapper mapper,
        ITopScorerQueryService topScorerQueryService,
        IPlayoffService playoffService)
    {
        _matchRepository = matchRepository;
        _mapper = mapper;
        _topScorerQueryService = topScorerQueryService;
        _playoffService = playoffService;
    }

    public List<MatchDto> GetScheduleByLeague(long leagueId)
    {
        _playoffService.EnsurePlayoffsGenerated(leagueId);
        var matches = _matchRepository.GetScheduleByLeague(leagueId);
        return matches.Results.Select(m => _mapper.Map<MatchDto>(m)).ToList();
    }

    public List<MatchDto> GetResultsByLeague(long leagueId)
    {
        _playoffService.EnsurePlayoffsGenerated(leagueId);
        var matches = _matchRepository.GetResultsByLeague(leagueId);
        return matches.Results.Select(_mapper.Map<MatchDto>).ToList();
    }

    public List<TopScorerDto> GetTopScorersByLeague(long leagueId)
    {
        var entries = _topScorerQueryService.GetTopScorersByLeague(leagueId);

        // Dodela pozicija
        for (int i = 0; i < entries.Count; i++)
        {
            if (i > 0 && entries[i].Goals == entries[i - 1].Goals)
                entries[i].Position = entries[i - 1].Position;
            else
                entries[i].Position = i + 1;
        }

        return entries;
    }
}
