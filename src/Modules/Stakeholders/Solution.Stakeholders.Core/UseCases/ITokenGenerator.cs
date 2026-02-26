using Solution.Stakeholders.API.Dtos;
using Solution.Stakeholders.Core.Domain;

namespace Solution.Stakeholders.Core.UseCases;

public interface ITokenGenerator
{
    AuthenticationTokensDto GenerateAccessToken(User user, long personId);
}