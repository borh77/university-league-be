using Solution.Identity.Core.Domain;
using Solution.Identity.Core.Domain.RepositoryInterfaces;

namespace Solution.Identity.Infrastructure.Database.Repositories;

public class UserDbRepository : IUserRepository
{
    private readonly IdentityContext _dbContext;

    public UserDbRepository(IdentityContext dbContext)
    {
        _dbContext = dbContext;
    }

    public User? GetByUsername(string username)
    {
        return _dbContext.Users.FirstOrDefault(u => u.Username == username);
    }
}
