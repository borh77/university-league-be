using Solution.Identity.Core.Domain;

namespace Solution.Identity.Tests.UnitTest;

public class UserUnitTests
{
    [Fact]
    public void Creates_successfully()
    {
        var user = new User("delegat", "hash", UserRole.Delegate, "Delegat na terenu");

        user.Username.ShouldBe("delegat");
        user.PasswordHash.ShouldBe("hash");
        user.Role.ShouldBe(UserRole.Delegate);
        user.FullName.ShouldBe("Delegat na terenu");
        user.IsActive.ShouldBeTrue();
        user.IsAdmin.ShouldBeFalse();
    }

    [Fact]
    public void Admin_user_is_admin()
    {
        var user = new User("admin", "hash", UserRole.Admin, "Administrator lige");

        user.IsAdmin.ShouldBeTrue();
    }

    [Fact]
    public void Fails_with_null_username()
    {
        Should.Throw<ArgumentException>(() =>
            new User(null!, "hash", UserRole.Delegate, "Delegat"));
    }

    [Fact]
    public void Fails_with_empty_username()
    {
        Should.Throw<ArgumentException>(() =>
            new User("", "hash", UserRole.Delegate, "Delegat"));
    }

    [Fact]
    public void Fails_with_whitespace_username()
    {
        Should.Throw<ArgumentException>(() =>
            new User("   ", "hash", UserRole.Delegate, "Delegat"));
    }

    [Fact]
    public void Fails_with_short_username()
    {
        Should.Throw<ArgumentException>(() =>
            new User("ab", "hash", UserRole.Delegate, "Delegat"));
    }

    [Fact]
    public void Fails_with_empty_password_hash()
    {
        Should.Throw<ArgumentException>(() =>
            new User("delegat", "", UserRole.Delegate, "Delegat"));
    }

    [Fact]
    public void Fails_with_null_full_name()
    {
        Should.Throw<ArgumentException>(() =>
            new User("delegat", "hash", UserRole.Delegate, null!));
    }

    [Fact]
    public void Fails_with_whitespace_full_name()
    {
        Should.Throw<ArgumentException>(() =>
            new User("delegat", "hash", UserRole.Delegate, "   "));
    }

    [Fact]
    public void Deactivate_sets_is_active_false()
    {
        var user = new User("delegat", "hash", UserRole.Delegate, "Delegat");

        user.Deactivate();

        user.IsActive.ShouldBeFalse();
    }

    [Fact]
    public void ChangePassword_updates_hash()
    {
        var user = new User("delegat", "old", UserRole.Delegate, "Delegat");

        user.ChangePassword("new");

        user.PasswordHash.ShouldBe("new");
    }

    [Fact]
    public void ChangePassword_rejects_empty_hash()
    {
        var user = new User("delegat", "old", UserRole.Delegate, "Delegat");

        Should.Throw<ArgumentException>(() => user.ChangePassword(""));
    }
}
