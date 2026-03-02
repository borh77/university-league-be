using Solution.BuildingBlocks.Core.Domain;

namespace Solution.UniLeague.Core.Domain;

public class Team : Entity
{
    public string Name { get; init; }
    public string? LogoUrl { get; init; }
    public IReadOnlyList<Player> Players { get; init; } = new List<Player>();

    private Team() { }
    public Team(string name, string? logoUrl)
    {
        if (string.IsNullOrWhiteSpace(name)) throw new ArgumentException("Name is required.");

        Name = name;
        LogoUrl = logoUrl;
    }
}