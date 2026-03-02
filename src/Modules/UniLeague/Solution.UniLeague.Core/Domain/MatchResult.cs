using Solution.BuildingBlocks.Core.Domain;
using System;
using System.Collections.Generic;

namespace Solution.UniLeague.Core.Domain;

public class MatchResult : ValueObject
{
    public int HomeScore { get; }
    public int AwayScore { get; }

    private MatchResult() { } // EF

    private MatchResult(int homeScore, int awayScore)
    {
        ValidateNonNegative(homeScore, nameof(homeScore));
        ValidateNonNegative(awayScore, nameof(awayScore));

        HomeScore = homeScore;
        AwayScore = awayScore;
    }

    public static MatchResult Create(int homeScore, int awayScore)
        => new MatchResult(homeScore, awayScore);

    public bool IsDraw() => HomeScore == AwayScore;

    public bool HomeWon() => HomeScore > AwayScore;

    public bool AwayWon() => AwayScore > HomeScore;

    private static void ValidateNonNegative(int value, string name)
    {
        if (value < 0)
            throw new ArgumentException($"{name} cannot be negative.");
    }

    protected override IEnumerable<object> GetEqualityComponents()
    {
        yield return HomeScore;
        yield return AwayScore;
    }

    public override string ToString() => $"{HomeScore}:{AwayScore}";
}