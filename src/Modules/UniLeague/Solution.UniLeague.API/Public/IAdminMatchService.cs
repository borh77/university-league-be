using Solution.UniLeague.API.Dtos;

namespace Solution.UniLeague.API.Public;

public interface IAdminMatchService
{
    AdminMatchDto ScheduleMatch(long leagueId, ScheduleMatchDto request);
    void UpdateMatch(long matchId, UpdateMatchDto request);
    void DeleteMatch(long matchId);
    void UpdateResult(long matchId, SubmitMatchResultDto request);
    void ClearResult(long matchId);
}
