using Solution.BuildingBlocks.Core.Domain;

namespace Solution.UniLeague.Core.Domain;

public class GoalEvent : ValueObject
{
    public string ScorerName { get; }
    public string TeamName { get; }
    public bool IsHomeTeamGoal { get; }
    public int Minute { get; }

    private GoalEvent() { } // EF

    private GoalEvent(string scorerName, string teamName, bool isHomeTeamGoal, int minute)
    {
        if (string.IsNullOrWhiteSpace(scorerName))
            throw new ArgumentException("ScorerName is required.", nameof(scorerName));
        if (string.IsNullOrWhiteSpace(teamName))
            throw new ArgumentException("TeamName is required.", nameof(teamName));
        if (minute <= 0)
            throw new ArgumentException("Minute must be positive.", nameof(minute));

        ScorerName = scorerName.Trim();
        TeamName = teamName.Trim();
        IsHomeTeamGoal = isHomeTeamGoal;
        Minute = minute;
    }

    public static GoalEvent Create(string scorerName, string teamName, bool isHomeTeamGoal, int minute)
        => new(scorerName, teamName, isHomeTeamGoal, minute);

    public override string ToString() => $"{Minute}' {ScorerName} ({TeamName})";

    protected override IEnumerable<object> GetEqualityComponents()
    {
        yield return ScorerName;
        yield return TeamName;
        yield return IsHomeTeamGoal;
        yield return Minute;
    }
}