namespace Solution.Identity.Core.Domain.RepositoryInterfaces;

public interface IUserRepository
{
    User? GetByUsername(string username);
}
