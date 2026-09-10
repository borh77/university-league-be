using AutoMapper;
using Moq;
using Solution.Identity.API.Dtos;
using Solution.Identity.Core.Domain;
using Solution.Identity.Core.Domain.RepositoryInterfaces;
using Solution.Identity.Core.Mappers;
using Solution.Identity.Core.UseCases;

namespace Solution.Identity.Tests.UnitTest;

public class AuthenticationServiceTests
{
    [Fact]
    public void Login_returns_token_for_valid_credentials()
    {
        var user = CreateUser(5, "admin", "stored-hash", UserRole.Admin, "Administrator lige");
        var expiresAt = new DateTime(2026, 1, 1, 12, 0, 0, DateTimeKind.Utc);

        var repo = new Mock<IUserRepository>();
        repo.Setup(r => r.GetByUsername("admin")).Returns(user);

        var hasher = new Mock<IPasswordHasher>();
        hasher.Setup(h => h.Verify("admin123", "stored-hash")).Returns(true);

        var tokens = new Mock<ITokenGenerator>();
        tokens.Setup(t => t.Generate(user)).Returns(new GeneratedToken("jwt-token", expiresAt));

        var result = CreateService(repo, hasher, tokens).Login(new LoginRequestDto
        {
            Username = "admin",
            Password = "admin123"
        });

        result.Token.ShouldBe("jwt-token");
        result.ExpiresAt.ShouldBe(expiresAt);
        result.Username.ShouldBe("admin");
        result.FullName.ShouldBe("Administrator lige");
        result.Role.ShouldBe("Admin");
    }

    [Fact]
    public void Login_trims_username_before_lookup()
    {
        var user = CreateUser(5, "admin", "stored-hash", UserRole.Admin, "Administrator lige");

        var repo = new Mock<IUserRepository>();
        repo.Setup(r => r.GetByUsername("admin")).Returns(user);

        var hasher = new Mock<IPasswordHasher>();
        hasher.Setup(h => h.Verify(It.IsAny<string>(), It.IsAny<string>())).Returns(true);

        var tokens = new Mock<ITokenGenerator>();
        tokens.Setup(t => t.Generate(It.IsAny<User>()))
            .Returns(new GeneratedToken("jwt-token", DateTime.UtcNow));

        var result = CreateService(repo, hasher, tokens).Login(new LoginRequestDto
        {
            Username = "  admin  ",
            Password = "admin123"
        });

        result.Token.ShouldBe("jwt-token");
        repo.Verify(r => r.GetByUsername("admin"), Times.Once);
    }

    [Fact]
    public void Login_throws_for_wrong_password()
    {
        var user = CreateUser(5, "admin", "stored-hash", UserRole.Admin, "Administrator lige");

        var repo = new Mock<IUserRepository>();
        repo.Setup(r => r.GetByUsername("admin")).Returns(user);

        var hasher = new Mock<IPasswordHasher>();
        hasher.Setup(h => h.Verify(It.IsAny<string>(), It.IsAny<string>())).Returns(false);

        var tokens = new Mock<ITokenGenerator>();

        Should.Throw<UnauthorizedAccessException>(() =>
            CreateService(repo, hasher, tokens).Login(new LoginRequestDto
            {
                Username = "admin",
                Password = "pogresna"
            }));

        tokens.Verify(t => t.Generate(It.IsAny<User>()), Times.Never);
    }

    [Fact]
    public void Login_throws_for_unknown_user()
    {
        var repo = new Mock<IUserRepository>();
        repo.Setup(r => r.GetByUsername(It.IsAny<string>())).Returns((User?)null);

        var hasher = new Mock<IPasswordHasher>();
        var tokens = new Mock<ITokenGenerator>();

        Should.Throw<UnauthorizedAccessException>(() =>
            CreateService(repo, hasher, tokens).Login(new LoginRequestDto
            {
                Username = "nepostojeci",
                Password = "bilosta"
            }));

        hasher.Verify(h => h.Verify(It.IsAny<string>(), It.IsAny<string>()), Times.Never);
    }

    [Fact]
    public void Login_throws_for_deactivated_user()
    {
        var user = CreateUser(5, "admin", "stored-hash", UserRole.Admin, "Administrator lige");
        user.Deactivate();

        var repo = new Mock<IUserRepository>();
        repo.Setup(r => r.GetByUsername("admin")).Returns(user);

        var hasher = new Mock<IPasswordHasher>();
        hasher.Setup(h => h.Verify(It.IsAny<string>(), It.IsAny<string>())).Returns(true);

        var tokens = new Mock<ITokenGenerator>();

        Should.Throw<UnauthorizedAccessException>(() =>
            CreateService(repo, hasher, tokens).Login(new LoginRequestDto
            {
                Username = "admin",
                Password = "admin123"
            }));

        tokens.Verify(t => t.Generate(It.IsAny<User>()), Times.Never);
    }

    [Fact]
    public void Login_throws_for_missing_password()
    {
        var repo = new Mock<IUserRepository>();
        var hasher = new Mock<IPasswordHasher>();
        var tokens = new Mock<ITokenGenerator>();

        Should.Throw<UnauthorizedAccessException>(() =>
            CreateService(repo, hasher, tokens).Login(new LoginRequestDto
            {
                Username = "admin",
                Password = ""
            }));

        repo.Verify(r => r.GetByUsername(It.IsAny<string>()), Times.Never);
    }

    // Helper

    private static AuthenticationService CreateService(
        Mock<IUserRepository> repo,
        Mock<IPasswordHasher> hasher,
        Mock<ITokenGenerator> tokens)
    {
        var mapper = new MapperConfiguration(cfg => cfg.AddProfile<IdentityProfile>())
            .CreateMapper();
        return new AuthenticationService(repo.Object, hasher.Object, tokens.Object, mapper);
    }

    private static User CreateUser(long id, string username, string passwordHash, UserRole role, string fullName)
    {
        var user = new User(username, passwordHash, role, fullName);

        typeof(User).BaseType!
            .GetProperty("Id")!
            .SetValue(user, id);

        return user;
    }
}
