using Solution.UniLeague.Core.Domain;
using Solution.UniLeague.Core.Domain.RepositoryInterfaces;

namespace Solution.UniLeague.Infrastructure.Database.Repositories;

public class StandingsDbRepository : IStandingsRepository
{
    private readonly UniLeagueContext _dbContext;

    public StandingsDbRepository(UniLeagueContext dbContext)
    {
        _dbContext = dbContext;
    }

    public void ReplaceForLeague(long leagueId, IReadOnlyCollection<StandingEntry> entries)
    {
        using var transaction = _dbContext.Database.BeginTransaction();

        var existing = _dbContext.StandingEntries
            .Where(s => s.LeagueId == leagueId)
            .ToList();

        _dbContext.StandingEntries.RemoveRange(existing);
        _dbContext.SaveChanges();

        _dbContext.StandingEntries.AddRange(entries);
        _dbContext.SaveChanges();

        transaction.Commit();
    }
}
