using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Solution.UniLeague.API.Dtos;
using Solution.UniLeague.API.Public;

namespace Solution.API.Controllers;

[ApiController]
[Route("api/admin/leagues")]
[Authorize(Roles = "Admin")]
public class AdminLeagueController : ControllerBase
{
    private readonly IAdminLeagueService _adminLeagueService;

    public AdminLeagueController(IAdminLeagueService adminLeagueService)
    {
        _adminLeagueService = adminLeagueService;
    }

    [HttpGet]
    public ActionResult<List<AdminLeagueDto>> GetAllLeagues()
        => Ok(_adminLeagueService.GetAllLeagues());

    [HttpGet("{leagueId:long}/teams")]
    public ActionResult<List<AdminTeamDto>> GetTeamsInLeague(long leagueId)
        => Ok(_adminLeagueService.GetTeamsInLeague(leagueId));

    [HttpPost("{leagueId:long}/teams")]
    public IActionResult AddTeamToLeague(long leagueId, [FromBody] AddTeamToLeagueDto request)
    {
        _adminLeagueService.AddTeamToLeague(leagueId, request.TeamId);
        return NoContent();
    }
}
