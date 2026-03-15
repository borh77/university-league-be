using Solution.BuildingBlocks.Core.Domain;

namespace Solution.UniLeague.Core.Domain;

public class SetScore : ValueObject
{
    public int SetNumber { get; }
    public int HomeScore { get; }
    public int AwayScore { get; }

    private SetScore() { } // EF 

    private SetScore(int setNumber, int homeScore, int awayScore)
    {
        if (setNumber <= 0)
            throw new ArgumentException("SetNumber must be positive.", nameof(setNumber));
        if (homeScore < 0)
            throw new ArgumentException("HomeScore cannot be negative.", nameof(homeScore));
        if (awayScore < 0)
            throw new ArgumentException("AwayScore cannot be negative.", nameof(awayScore));

        SetNumber = setNumber;
        HomeScore = homeScore;
        AwayScore = awayScore;
    }

    public static SetScore Create(int setNumber, int homeScore, int awayScore)
        => new(setNumber, homeScore, awayScore);

    public bool HomeWonSet() => HomeScore > AwayScore;
    public bool AwayWonSet() => AwayScore > HomeScore;

    public override string ToString() => $"S{SetNumber}: {HomeScore}:{AwayScore}";

    protected override IEnumerable<object> GetEqualityComponents()
    {
        yield return SetNumber;
        yield return HomeScore;
        yield return AwayScore;
    }
}