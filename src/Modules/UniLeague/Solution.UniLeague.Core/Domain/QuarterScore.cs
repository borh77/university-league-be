using Solution.BuildingBlocks.Core.Domain;

namespace Solution.UniLeague.Core.Domain;

public class QuarterScore : ValueObject
{
    public int QuarterNumber { get; }
    public int HomeScore { get; }
    public int AwayScore { get; }

    private QuarterScore() { } // EF Core

    private QuarterScore(int quarterNumber, int homeScore, int awayScore)
    {
        if (quarterNumber <= 0)
            throw new ArgumentException("QuarterNumber must be positive.", nameof(quarterNumber));
        if (homeScore < 0)
            throw new ArgumentException("HomeScore cannot be negative.", nameof(homeScore));
        if (awayScore < 0)
            throw new ArgumentException("AwayScore cannot be negative.", nameof(awayScore));

        QuarterNumber = quarterNumber;
        HomeScore = homeScore;
        AwayScore = awayScore;
    }

    public static QuarterScore Create(int quarterNumber, int homeScore, int awayScore)
        => new(quarterNumber, homeScore, awayScore);

    public override string ToString() => $"Q{QuarterNumber}: {HomeScore}:{AwayScore}";

    protected override IEnumerable<object> GetEqualityComponents()
    {
        yield return QuarterNumber;
        yield return HomeScore;
        yield return AwayScore;
    }
}