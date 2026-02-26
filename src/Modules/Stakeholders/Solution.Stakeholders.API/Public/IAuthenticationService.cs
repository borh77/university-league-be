using Solution.Stakeholders.API.Dtos;

namespace Solution.Stakeholders.API.Public;

public interface IAuthenticationService
{
    AuthenticationTokensDto Login(CredentialsDto credentials);
    AuthenticationTokensDto RegisterTourist(AccountRegistrationDto account);
}