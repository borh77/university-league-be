using Shouldly;
using Solution.UniLeague.Core.Domain;

namespace Solution.UniLeague.Tests.Unit.Domain;

public class MatchResultTests
{
    [Fact]
    public void Creates_successfully()
    {
        var result = MatchResult.Create(2, 1);

        result.HomeScore.ShouldBe(2);
        result.AwayScore.ShouldBe(1);
    }

    [Fact]
    public void Creates_with_zero_scores()
    {
        var result = MatchResult.Create(0, 0);

        result.HomeScore.ShouldBe(0);
        result.AwayScore.ShouldBe(0);
    }

    [Fact]
    public void ToString_returns_correct_format()
    {
        MatchResult.Create(2, 1).ToString().ShouldBe("2:1");
        MatchResult.Create(0, 0).ToString().ShouldBe("0:0");
        MatchResult.Create(10, 3).ToString().ShouldBe("10:3");
    }

    [Fact]
    public void Fails_with_negative_home_score()
    {
        Should.Throw<ArgumentException>(() => MatchResult.Create(-1, 0));
    }

    [Fact]
    public void Fails_with_negative_away_score()
    {
        Should.Throw<ArgumentException>(() => MatchResult.Create(0, -1));
    }
}