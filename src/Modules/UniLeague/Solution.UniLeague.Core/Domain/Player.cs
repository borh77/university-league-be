using Solution.BuildingBlocks.Core.Domain;

namespace Solution.UniLeague.Core.Domain;

public class Player : Entity
{
    public long TeamId { get; init; }
    public string FirstName { get; init; }
    public string LastName { get; init; }
    public int JerseyNumber { get; init; }
    public string? ImageUrl { get; init; }

    // Parameterless constructor for EF Core
    private Player() { }

    public Player(long teamId, string firstName, string lastName, int jerseyNumber, string? imageUrl)
    {
        if (teamId <= 0) throw new ArgumentException("Invalid TeamId.");
        if (string.IsNullOrWhiteSpace(firstName)) throw new ArgumentException("FirstName is required.");
        if (string.IsNullOrWhiteSpace(lastName)) throw new ArgumentException("LastName is required.");
        if (jerseyNumber < 1 || jerseyNumber > 99) throw new ArgumentException("JerseyNumber must be between 1 and 99.");

        TeamId = teamId;
        FirstName = firstName;
        LastName = lastName;
        JerseyNumber = jerseyNumber;
        ImageUrl = imageUrl;
    }
}