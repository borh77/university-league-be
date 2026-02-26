using Solution.UniLeague.API.Public;
using Microsoft.AspNetCore.Mvc;

namespace Solution.API.Controllers;

/// <summary>
/// Placeholder controller for the UniLeague module.
/// Replace with real controllers (Teams, Players, Matches, etc.) in Phase 1.
/// </summary>
[ApiController]
[Route("api/[controller]")]
public class UniLeagueController : ControllerBase
{
    private readonly IHealthService _healthService;

    public UniLeagueController(IHealthService healthService)
    {
        _healthService = healthService;
    }

    /// <summary>GET /api/unileague/ping – sanity check that the module is wired up.</summary>
    [HttpGet("ping")]
    public ActionResult<string> Ping() => Ok(_healthService.Ping());
}
