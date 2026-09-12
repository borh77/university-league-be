using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Solution.UniLeague.API.Dtos;
using Solution.UniLeague.API.Public;

namespace Solution.API.Controllers;

[ApiController]
[Authorize(Roles = "Admin")]
public class AdminMatchController : ControllerBase
{
    private readonly IAdminMatchService _adminMatchService;

    public AdminMatchController(IAdminMatchService adminMatchService)
    {
        _adminMatchService = adminMatchService;
    }

    [HttpPost("api/admin/leagues/{leagueId:long}/matches")]
    public ActionResult<AdminMatchDto> ScheduleMatch(long leagueId, [FromBody] ScheduleMatchDto request)
    {
        var match = _adminMatchService.ScheduleMatch(leagueId, request);
        return Ok(match);
    }

    [HttpPut("api/admin/matches/{matchId:long}")]
    public IActionResult UpdateMatch(long matchId, [FromBody] UpdateMatchDto request)
    {
        _adminMatchService.UpdateMatch(matchId, request);
        return NoContent();
    }

    [HttpDelete("api/admin/matches/{matchId:long}")]
    public IActionResult DeleteMatch(long matchId)
    {
        _adminMatchService.DeleteMatch(matchId);
        return NoContent();
    }

    [HttpPut("api/admin/matches/{matchId:long}/result")]
    public IActionResult UpdateResult(long matchId, [FromBody] SubmitMatchResultDto request)
    {
        _adminMatchService.UpdateResult(matchId, request);
        return NoContent();
    }

    [HttpDelete("api/admin/matches/{matchId:long}/result")]
    public IActionResult ClearResult(long matchId)
    {
        _adminMatchService.ClearResult(matchId);
        return NoContent();
    }
}
