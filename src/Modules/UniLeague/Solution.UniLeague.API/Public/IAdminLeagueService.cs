using Solution.UniLeague.API.Dtos;

namespace Solution.UniLeague.API.Public;

public interface IAdminLeagueService
{
    List<AdminLeagueDto> GetAllLeagues();
    List<AdminTeamDto> GetTeamsInLeague(long leagueId);
    List<AdminTeamDto> GetAllTeams();
    void AddTeamToLeague(long leagueId, long teamId);
}
