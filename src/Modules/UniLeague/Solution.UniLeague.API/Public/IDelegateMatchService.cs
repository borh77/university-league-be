using Solution.UniLeague.API.Dtos;

namespace Solution.UniLeague.API.Public;

public interface IDelegateMatchService
{
    DelegateMatchDto GetMatchForEntry(long matchId);
    void SubmitResult(long matchId, SubmitMatchResultDto request);
}
