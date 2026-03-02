using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Solution.UniLeague.API.Dtos;
using Solution.UniLeague.API.Public;

namespace Solution.API.Controllers.Public;

[AllowAnonymous]
[Route("api/public/leagues/{leagueId:long}")]
[ApiController]
public class ScheduleController : ControllerBase
{
    private readonly ILeagueService _leaguePublicService;

    public ScheduleController(ILeagueService leaguePublicService)
    {
        _leaguePublicService = leaguePublicService;
    }

    [HttpGet("schedule")]
    public ActionResult<List<MatchDto>> GetSchedule(long leagueId)
    {
        return Ok(_leaguePublicService.GetScheduleByLeague(leagueId));
    }

    [HttpGet("results")]
    public ActionResult<List<MatchDto>> GetResults(long leagueId)
    {
        return Ok(_leaguePublicService.GetResultsByLeague(leagueId));
    }
}