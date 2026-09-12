using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Solution.UniLeague.API.Dtos;
using Solution.UniLeague.API.Public;

namespace Solution.API.Controllers;

[ApiController]
[Route("api/admin/teams")]
[Authorize(Roles = "Admin")]
public class AdminTeamController : ControllerBase
{
    private readonly IAdminLeagueService _adminLeagueService;

    public AdminTeamController(IAdminLeagueService adminLeagueService)
    {
        _adminLeagueService = adminLeagueService;
    }

    [HttpGet]
    public ActionResult<List<AdminTeamDto>> GetAllTeams()
        => Ok(_adminLeagueService.GetAllTeams());
}
