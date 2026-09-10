using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Solution.UniLeague.API.Dtos;
using Solution.UniLeague.API.Public;

namespace Solution.API.Controllers;

[ApiController]
[Route("api/delegate/matches")]
[Authorize(Roles = "Delegate,Admin")]
public class DelegateController : ControllerBase
{
    private readonly IDelegateMatchService _delegateMatchService;

    public DelegateController(IDelegateMatchService delegateMatchService)
    {
        _delegateMatchService = delegateMatchService;
    }

    [HttpGet("{matchId:long}")]
    public ActionResult<DelegateMatchDto> GetMatch(long matchId)
        => Ok(_delegateMatchService.GetMatchForEntry(matchId));

    [HttpPut("{matchId:long}/result")]
    public IActionResult SubmitResult(long matchId, [FromBody] SubmitMatchResultDto request)
    {
        _delegateMatchService.SubmitResult(matchId, request);
        return NoContent();
    }
}
