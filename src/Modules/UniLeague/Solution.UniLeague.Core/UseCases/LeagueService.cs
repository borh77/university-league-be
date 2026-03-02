using AutoMapper;
using Solution.UniLeague.API.Dtos;
using Solution.UniLeague.API.Public;
using Solution.UniLeague.Core.Domain.RepositoryInterfaces;

namespace Solution.UniLeague.Core.UseCases;

public class LeagueService : ILeagueService
{
    private readonly IMatchRepository _matchRepository;
    private readonly IMapper _mapper;

    public LeagueService(IMatchRepository matchRepository, IMapper mapper)
    {
        _matchRepository = matchRepository;
        _mapper = mapper;
    }

    public List<MatchDto> GetScheduleByLeague(long leagueId)
    {
        var matches = _matchRepository.GetScheduleByLeague(leagueId);
        return matches.Results.Select(m => _mapper.Map<MatchDto>(m)).ToList();
    }

    public List<MatchDto> GetResultsByLeague(long leagueId)
    {
        var matches = _matchRepository.GetResultsByLeague(leagueId);
        return matches.Results.Select(_mapper.Map<MatchDto>).ToList();
    }
}