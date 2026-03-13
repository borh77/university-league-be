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

    [Fact]
    public void Create_without_quarters_has_no_quarters()
    {
        var result = MatchResult.Create(3, 1);
        result.HasQuarters.ShouldBeFalse();
        result.Quarters.ShouldBeEmpty();
    }

    [Fact]
    public void CreateWithQuarters_creates_successfully()
    {
        var quarters = new[]
        {
            QuarterScore.Create(1, 25, 21),
            QuarterScore.Create(2, 22, 25),
            QuarterScore.Create(3, 30, 28),
            QuarterScore.Create(4, 25, 21),
        };

        var result = MatchResult.CreateWithQuarters(102, 95, quarters);

        result.HomeScore.ShouldBe(102);
        result.AwayScore.ShouldBe(95);
        result.HasQuarters.ShouldBeTrue();
        result.Quarters.Count.ShouldBe(4);
    }

    [Fact]
    public void CreateWithQuarters_sorts_quarters_chronologically()
    {
        //namerno unesen van reda
        var quarters = new[]
        {
            QuarterScore.Create(3, 30, 28),
            QuarterScore.Create(1, 25, 21),
            QuarterScore.Create(4, 25, 21),
            QuarterScore.Create(2, 22, 25),
        };

        var result = MatchResult.CreateWithQuarters(102, 95, quarters);

        result.Quarters[0].QuarterNumber.ShouldBe(1);
        result.Quarters[1].QuarterNumber.ShouldBe(2);
        result.Quarters[2].QuarterNumber.ShouldBe(3);
        result.Quarters[3].QuarterNumber.ShouldBe(4);
    }

    [Fact]
    public void CreateWithQuarters_fails_when_sum_does_not_match_total()
    {
        var quarters = new[]
        {
            QuarterScore.Create(1, 25, 21),
            QuarterScore.Create(2, 22, 25),
        };

        //ukupno tvrdi 100:90 ali zbir je 47:46 — neispravno
        Should.Throw<ArgumentException>(() =>
            MatchResult.CreateWithQuarters(100, 90, quarters));
    }

    [Fact]
    public void CreateWithQuarters_fails_when_quarter_numbers_not_sequential()
    {
        //preskače Q2
        var quarters = new[]
        {
            QuarterScore.Create(1, 25, 21),
            QuarterScore.Create(3, 30, 28),
        };

        Should.Throw<ArgumentException>(() =>
            MatchResult.CreateWithQuarters(55, 49, quarters));
    }

    [Fact]
    public void CreateWithQuarters_fails_with_empty_list()
    {
        Should.Throw<ArgumentException>(() =>
            MatchResult.CreateWithQuarters(10, 8, Array.Empty<QuarterScore>()));
    }

    [Fact]
    public void CreateWithQuarters_fails_with_null()
    {
        Should.Throw<ArgumentNullException>(() =>
            MatchResult.CreateWithQuarters(10, 8, null!));
    }

    [Fact]
    public void ToString_returns_total_score_regardless_of_quarters()
    {
        var quarters = new[]
        {
            QuarterScore.Create(1, 25, 21),
            QuarterScore.Create(2, 22, 25),
            QuarterScore.Create(3, 30, 28),
            QuarterScore.Create(4, 25, 21),
        };

        MatchResult.CreateWithQuarters(102, 95, quarters).ToString().ShouldBe("102:95");
    }

    [Fact]
    public void Create_quarter_successfully()
    {
        var q = QuarterScore.Create(1, 25, 21);
        q.QuarterNumber.ShouldBe(1);
        q.HomeScore.ShouldBe(25);
        q.AwayScore.ShouldBe(21);
    }

    [Fact]
    public void Fails_with_zero_quarter_number()
    {
        Should.Throw<ArgumentException>(() => QuarterScore.Create(0, 10, 8));
    }

    [Fact]
    public void Fails_with_negative_quarter_number()
    {
        Should.Throw<ArgumentException>(() => QuarterScore.Create(-1, 10, 8));
    }

    [Fact]
    public void Fails_quarter_with_negative_home_score()
    {
        Should.Throw<ArgumentException>(() => QuarterScore.Create(1, -1, 8));
    }

    [Fact]
    public void Fails_quarter_with_negative_away_score()
    {
        Should.Throw<ArgumentException>(() => QuarterScore.Create(1, 10, -1));
    }

    [Fact]
    public void ToString_quarter_returns_correct_format()
    {
        QuarterScore.Create(1, 25, 21).ToString().ShouldBe("Q1: 25:21");
    }


    [Fact]
    public void Creates_set_successfully()
    {
        var s = SetScore.Create(1, 25, 21);
        s.SetNumber.ShouldBe(1);
        s.HomeScore.ShouldBe(25);
        s.AwayScore.ShouldBe(21);
    }

    [Fact]
    public void Fails_with_zero_set_number()
    {
        Should.Throw<ArgumentException>(() => SetScore.Create(0, 25, 21));
    }

    [Fact]
    public void Fails_with_negative_set_number()
    {
        Should.Throw<ArgumentException>(() => SetScore.Create(-1, 25, 21));
    }

    [Fact]
    public void Fails_sets_with_negative_home_score()
    {
        Should.Throw<ArgumentException>(() => SetScore.Create(1, -1, 21));
    }

    [Fact]
    public void Fails_sets_with_negative_away_score()
    {
        Should.Throw<ArgumentException>(() => SetScore.Create(1, 25, -1));
    }

    [Fact]
    public void HomeWonSet_returns_true_when_home_score_higher()
    {
        SetScore.Create(1, 25, 21).HomeWonSet().ShouldBeTrue();
        SetScore.Create(1, 25, 21).AwayWonSet().ShouldBeFalse();
    }

    [Fact]
    public void AwayWonSet_returns_true_when_away_score_higher()
    {
        SetScore.Create(2, 22, 25).AwayWonSet().ShouldBeTrue();
        SetScore.Create(2, 22, 25).HomeWonSet().ShouldBeFalse();
    }

    [Fact]
    public void ToString_set_returns_correct_format()
    {
        SetScore.Create(1, 25, 21).ToString().ShouldBe("S1: 25:21");
    }
}    