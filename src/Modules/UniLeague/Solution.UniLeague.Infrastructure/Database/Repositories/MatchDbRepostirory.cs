using Microsoft.EntityFrameworkCore;
using Solution.BuildingBlocks.Core.UseCases;
using Solution.BuildingBlocks.Infrastructure.Database;
using Solution.UniLeague.Core.Domain;
using Solution.UniLeague.Core.Domain.RepositoryInterfaces;

namespace Solution.UniLeague.Infrastructure.Database.Repositories;

public class MatchDbRepository : IMatchRepository
{
    private readonly UniLeagueContext _dbContext;

    public MatchDbRepository(UniLeagueContext dbContext)
    {
        _dbContext = dbContext;
    }

    public PagedResult<Match> GetScheduleByLeague(long leagueId)
    {
        var task = _dbContext.Matches
            .Where(m => m.LeagueId == leagueId)
            .OrderBy(m => m.RoundNumber)
            .ThenBy(m => m.ScheduledAt)
            .GetPaged(0, 0);
        task.Wait();
        return task.Result;
    }
}