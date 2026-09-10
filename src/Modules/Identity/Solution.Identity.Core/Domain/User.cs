using Solution.BuildingBlocks.Core.Domain;

namespace Solution.Identity.Core.Domain;

public class User : Entity
{
    public string Username { get; private set; }
    public string PasswordHash { get; private set; }
    public UserRole Role { get; private set; }
    public string FullName { get; private set; }
    public bool IsActive { get; private set; } = true;

    // Parameterless constructor for EF Core
    private User() { }

    public User(string username, string passwordHash, UserRole role, string fullName)
    {
        if (string.IsNullOrWhiteSpace(username)) throw new ArgumentException("Username is required.");
        if (username.Trim().Length < 3) throw new ArgumentException("Username must be at least 3 characters.");
        if (string.IsNullOrWhiteSpace(passwordHash)) throw new ArgumentException("PasswordHash is required.");
        if (string.IsNullOrWhiteSpace(fullName)) throw new ArgumentException("FullName is required.");

        Username = username;
        PasswordHash = passwordHash;
        Role = role;
        FullName = fullName;
    }

    public bool IsAdmin => Role == UserRole.Admin;

    public void Deactivate() => IsActive = false;

    public void ChangePassword(string newHash)
    {
        if (string.IsNullOrWhiteSpace(newHash)) throw new ArgumentException("PasswordHash is required.");
        PasswordHash = newHash;
    }
}
