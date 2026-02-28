using Solution.UniLeague.API.Dtos;

namespace Solution.UniLeague.API.Public;

public interface ILeagueService
{
    List<MatchDto> GetScheduleByLeague(long leagueId);
}