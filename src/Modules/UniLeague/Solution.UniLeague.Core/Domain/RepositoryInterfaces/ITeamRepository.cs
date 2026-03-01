namespace Solution.UniLeague.Core.Domain.RepositoryInterfaces;

public interface ITeamRepository
{
    public Team GetByIdWithPlayers(long teamId);
}