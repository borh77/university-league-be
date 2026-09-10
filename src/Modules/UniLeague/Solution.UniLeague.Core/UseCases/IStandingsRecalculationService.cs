namespace Solution.UniLeague.Core.UseCases;

public interface IStandingsRecalculationService
{
    // Prebriše i ponovo izgradi tabelu lige iz svih odigranih regularnih mečeva. Idempotentno.
    void Recalculate(long leagueId);
}
