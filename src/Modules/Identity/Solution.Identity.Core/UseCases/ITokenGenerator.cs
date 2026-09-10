using Solution.Identity.Core.Domain;

namespace Solution.Identity.Core.UseCases;

public interface ITokenGenerator
{
    GeneratedToken Generate(User user);
}
