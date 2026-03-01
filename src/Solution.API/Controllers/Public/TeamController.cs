using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Solution.BuildingBlocks.Core.Exceptions;
using Solution.UniLeague.API.Dtos;
using Solution.UniLeague.API.Public;

namespace Solution.API.Controllers.Public;

[AllowAnonymous]
[Route("api/public/teams")]
[ApiController]
public class TeamController : ControllerBase
{
    private readonly ITeamService _teamPublicService;

    public TeamController(ITeamService teamPublicService)
    {
        _teamPublicService = teamPublicService;
    }

    [HttpGet("{teamId:long}")]
    public ActionResult<TeamProfileDto> GetTeamProfile(long teamId)
    {
        try
        {
            return Ok(_teamPublicService.GetTeamProfile(teamId));
        }
        catch (NotFoundException)
        {
            return NotFound();
        }
    }
}