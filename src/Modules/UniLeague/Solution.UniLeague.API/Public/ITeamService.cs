using Solution.UniLeague.API.Dtos;

namespace Solution.UniLeague.API.Public;

public interface ITeamService
{
    TeamProfileDto GetTeamProfile(long teamId);
}