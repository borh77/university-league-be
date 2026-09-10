using Shouldly;
using Solution.UniLeague.Core.Domain;

namespace Solution.UniLeague.Tests.Unit.Domain;

public class PlayerStatLineTests
{
    [Fact]
    public void Creates_successfully()
    {
        var line = PlayerStatLine.Create(5, "Marko Marković", 10, isHomeTeam: true, points: 22);

        line.PlayerId.ShouldBe(5);
        line.PlayerName.ShouldBe("Marko Marković");
        line.JerseyNumber.ShouldBe(10);
        line.IsHomeTeam.ShouldBeTrue();
        line.Points.ShouldBe(22);
    }

    [Fact]
    public void Trims_player_name()
    {
        PlayerStatLine.Create(5, "  Marko Marković  ", 10, true, 22).PlayerName.ShouldBe("Marko Marković");
    }

    [Fact]
    public void Allows_zero_points()
    {
        PlayerStatLine.Create(5, "Marko", 10, true, 0).Points.ShouldBe(0);
    }

    [Fact]
    public void Fails_with_zero_player_id()
    {
        Should.Throw<ArgumentException>(() => PlayerStatLine.Create(0, "Marko", 10, true, 5));
    }

    [Fact]
    public void Allows_negative_player_id_for_seed_data()
    {
        PlayerStatLine.Create(-2001, "Marko", 10, true, 5).PlayerId.ShouldBe(-2001);
    }

    [Fact]
    public void Fails_with_empty_name()
    {
        Should.Throw<ArgumentException>(() => PlayerStatLine.Create(5, "   ", 10, true, 5));
    }

    [Fact]
    public void Fails_with_jersey_out_of_range()
    {
        Should.Throw<ArgumentException>(() => PlayerStatLine.Create(5, "Marko", 0, true, 5));
        Should.Throw<ArgumentException>(() => PlayerStatLine.Create(5, "Marko", 100, true, 5));
    }

    [Fact]
    public void Fails_with_negative_points()
    {
        Should.Throw<ArgumentException>(() => PlayerStatLine.Create(5, "Marko", 10, true, -1));
    }

    [Fact]
    public void ToString_returns_readable_format()
    {
        PlayerStatLine.Create(5, "Marko", 10, true, 22).ToString().ShouldBe("#10 Marko: 22");
    }

    // ── MatchResult.CreateWithPlayerStats ─────────────────────────────

    [Fact]
    public void CreateWithPlayerStats_basketball_creates_when_sums_match_total()
    {
        var stats = new[]
        {
            PlayerStatLine.Create(1, "H1", 4, true, 60),
            PlayerStatLine.Create(2, "H2", 5, true, 42),
            PlayerStatLine.Create(3, "A1", 6, false, 50),
            PlayerStatLine.Create(4, "A2", 7, false, 45),
        };

        var quarters = new[]
        {
            QuarterScore.Create(1, 25, 21),
            QuarterScore.Create(2, 22, 25),
            QuarterScore.Create(3, 30, 28),
            QuarterScore.Create(4, 25, 21),
        };

        var result = MatchResult.CreateWithPlayerStats(102, 95, stats, quarters: quarters);

        result.HomeScore.ShouldBe(102);
        result.AwayScore.ShouldBe(95);
        result.HasPlayerStats.ShouldBeTrue();
        result.HasQuarters.ShouldBeTrue();
        result.PlayerStats.Count.ShouldBe(4);
    }

    [Fact]
    public void CreateWithPlayerStats_fails_when_home_points_do_not_match_total()
    {
        var stats = new[]
        {
            PlayerStatLine.Create(1, "H1", 4, true, 50), // zbir domacih 50, a skor kaze 102
            PlayerStatLine.Create(3, "A1", 6, false, 95),
        };

        Should.Throw<ArgumentException>(() =>
            MatchResult.CreateWithPlayerStats(102, 95, stats));
    }

    [Fact]
    public void CreateWithPlayerStats_fails_when_away_points_do_not_match_total()
    {
        var stats = new[]
        {
            PlayerStatLine.Create(1, "H1", 4, true, 102),
            PlayerStatLine.Create(3, "A1", 6, false, 40), // treba 95
        };

        Should.Throw<ArgumentException>(() =>
            MatchResult.CreateWithPlayerStats(102, 95, stats));
    }

    [Fact]
    public void CreateWithPlayerStats_volleyball_validates_against_set_points_not_set_count()
    {
        var sets = new[]
        {
            SetScore.Create(1, 25, 21),
            SetScore.Create(2, 22, 25),
            SetScore.Create(3, 25, 18),
            SetScore.Create(4, 25, 19),
        };
        // ukupno poena: domaci 97, gosti 83; skor u setovima 3:1

        var stats = new[]
        {
            PlayerStatLine.Create(1, "H1", 4, true, 55),
            PlayerStatLine.Create(2, "H2", 5, true, 42),
            PlayerStatLine.Create(3, "A1", 6, false, 40),
            PlayerStatLine.Create(4, "A2", 7, false, 43),
        };

        var result = MatchResult.CreateWithPlayerStats(3, 1, stats, sets: sets);

        result.HomeScore.ShouldBe(3);
        result.AwayScore.ShouldBe(1);
        result.HasSets.ShouldBeTrue();
        result.HasPlayerStats.ShouldBeTrue();
    }

    [Fact]
    public void CreateWithPlayerStats_volleyball_fails_when_points_do_not_match_set_points()
    {
        var sets = new[]
        {
            SetScore.Create(1, 25, 21),
            SetScore.Create(2, 25, 18),
            SetScore.Create(3, 25, 19),
        };
        // domaci treba 75 poena

        var stats = new[]
        {
            PlayerStatLine.Create(1, "H1", 4, true, 50),
            PlayerStatLine.Create(3, "A1", 6, false, 58),
        };

        Should.Throw<ArgumentException>(() =>
            MatchResult.CreateWithPlayerStats(3, 0, stats, sets: sets));
    }

    [Fact]
    public void CreateWithPlayerStats_fails_with_empty_list()
    {
        Should.Throw<ArgumentException>(() =>
            MatchResult.CreateWithPlayerStats(0, 0, Array.Empty<PlayerStatLine>()));
    }

    [Fact]
    public void CreateWithPlayerStats_fails_with_null()
    {
        Should.Throw<ArgumentNullException>(() =>
            MatchResult.CreateWithPlayerStats(0, 0, null!));
    }
}
