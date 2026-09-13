using Solution.UniLeague.API.Dtos;

namespace Solution.UniLeague.API.Public;

public interface ILeagueService
{
    List<MatchDto> GetScheduleByLeague(long leagueId);
    List<MatchDto> GetResultsByLeague(long leagueId);
    List<TopScorerDto> GetTopScorersByLeague(long leagueId);
    List<PublicLeagueDto> GetAllLeagues();
}