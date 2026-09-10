using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Solution.Identity.API.Dtos;
using Solution.Identity.API.Public;

namespace Solution.API.Controllers;

[ApiController]
[Route("api/auth")]
public class AuthController : ControllerBase
{
    private readonly IAuthenticationService _authenticationService;

    public AuthController(IAuthenticationService authenticationService)
    {
        _authenticationService = authenticationService;
    }

    [AllowAnonymous]
    [HttpPost("login")]
    public ActionResult<AuthResponseDto> Login([FromBody] LoginRequestDto request)
        => Ok(_authenticationService.Login(request));

    [Authorize]
    [HttpGet("me")]
    public ActionResult<CurrentUserDto> Me()
    {
        long.TryParse(User.FindFirstValue("sub"), out var id);

        return Ok(new CurrentUserDto
        {
            Id = id,
            Username = User.FindFirstValue("unique_name") ?? User.Identity?.Name ?? string.Empty,
            FullName = User.FindFirstValue("name") ?? string.Empty,
            Role = User.FindFirstValue(ClaimTypes.Role) ?? string.Empty
        });
    }
}
