using Solution.BuildingBlocks.Core.Domain;

namespace Solution.UniLeague.Core.Domain;

public class League : Entity
{
    public Sport Sport { get; private set; }
    public Gender? LeagueGender { get; private set; }

    private readonly List<StandingEntry> _standings = new();
    public IReadOnlyCollection<StandingEntry> Standings => _standings.AsReadOnly();

    private League() { }

    public League(Sport sport, Gender? leagueGender = null)
    {
        if (sport == Sport.Volleyball && leagueGender is null)
            throw new ArgumentException("Gender must be specified for Volleyball leagues.");

        Sport = sport;
        LeagueGender = sport == Sport.Volleyball ? leagueGender : null;
    }
}