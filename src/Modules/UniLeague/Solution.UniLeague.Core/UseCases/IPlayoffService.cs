namespace Solution.UniLeague.Core.UseCases;

public interface IPlayoffService
{
    void EnsurePlayoffsGenerated(long leagueId);

    // Poziva se posle svake izmene regularnog rezultata - odlucuje sta biva sa vec generisanim plej-ofom
    void HandleRegularSeasonResultChanged(long leagueId);
}
