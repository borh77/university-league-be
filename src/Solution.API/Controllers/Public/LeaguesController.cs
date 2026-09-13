using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Solution.UniLeague.API.Dtos;
using Solution.UniLeague.API.Public;

namespace Solution.API.Controllers.Public;

[AllowAnonymous]
[Route("api/public/leagues")]
[ApiController]
public class LeaguesController : ControllerBase
{
    private readonly ILeagueService _leagueService;

    public LeaguesController(ILeagueService leagueService)
    {
        _leagueService = leagueService;
    }

    [HttpGet]
    public ActionResult<List<PublicLeagueDto>> GetAllLeagues()
        => Ok(_leagueService.GetAllLeagues());
}
