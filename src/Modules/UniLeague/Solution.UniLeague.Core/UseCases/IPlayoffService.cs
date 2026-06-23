namespace Solution.UniLeague.Core.UseCases;

public interface IPlayoffService
{
    void EnsurePlayoffsGenerated(long leagueId);
}
