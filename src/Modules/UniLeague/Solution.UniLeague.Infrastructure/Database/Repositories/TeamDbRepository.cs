using Microsoft.EntityFrameworkCore;
using Solution.BuildingBlocks.Core.Exceptions;
using Solution.UniLeague.Core.Domain;
using Solution.UniLeague.Core.Domain.RepositoryInterfaces;

namespace Solution.UniLeague.Infrastructure.Database.Repositories;

public class TeamDbRepository : ITeamRepository
{
    private readonly UniLeagueContext _dbContext;

    public TeamDbRepository(UniLeagueContext dbContext)
    {
        _dbContext = dbContext;
    }

    public Team GetByIdWithPlayers(long teamId)
    {
        var team = _dbContext.Teams
            .Include(t => t.Players)
            .FirstOrDefault(t => t.Id == teamId);

        if (team is null)
            throw new NotFoundException($"Team with id {teamId} was not found.");

        return team;
    }
}