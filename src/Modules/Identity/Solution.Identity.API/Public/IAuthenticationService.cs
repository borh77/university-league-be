using Solution.Identity.API.Dtos;

namespace Solution.Identity.API.Public;

public interface IAuthenticationService
{
    AuthResponseDto Login(LoginRequestDto request);
}
