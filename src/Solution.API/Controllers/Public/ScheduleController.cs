using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Solution.UniLeague.API.Dtos;
using Solution.UniLeague.API.Public;

namespace Solution.API.Controllers.Public;

[AllowAnonymous]
[Route("api/public/leagues/{leagueId:long}/schedule")]
[ApiController]
public class ScheduleController : ControllerBase
{
    private readonly ILeagueService _leaguePublicService;

    public ScheduleController(ILeagueService leaguePublicService)
    {
        _leaguePublicService = leaguePublicService;
    }

    [HttpGet]
    public ActionResult<List<MatchDto>> GetSchedule(long leagueId)
    {
        return Ok(_leaguePublicService.GetScheduleByLeague(leagueId));
    }
}