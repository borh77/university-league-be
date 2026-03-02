using Microsoft.AspNetCore.Mvc;
using Solution.BuildingBlocks.Core.Exceptions;
using Solution.UniLeague.API.Dtos;
using Solution.UniLeague.API.Public;

namespace Solution.API.Controllers.Public;

[Route("api/public/{sport}/standings")]
[ApiController]
public class StandingsController : ControllerBase
{
    private readonly IStandingsService _standingsService;

    public StandingsController(IStandingsService standingsService)
    {
        _standingsService = standingsService;
    }

    [HttpGet]
    public ActionResult<List<StandingsRowDto>> GetStandings(
        [FromRoute] string sport,
        [FromQuery] string? gender)
    {
        var result = _standingsService.GetStandings(sport, gender);
        return Ok(result);
    }
}