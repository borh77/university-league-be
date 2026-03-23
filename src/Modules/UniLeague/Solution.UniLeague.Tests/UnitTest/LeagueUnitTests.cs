using Shouldly;
using Solution.UniLeague.Core.Domain;

namespace Solution.UniLeague.Tests.Unit;

public class LeagueUnitTests
{
    // ── Kreiranje liga ────────────────────────────────────────────────

    [Fact]
    public void Creates_football_league_successfully()
    {
        var league = new League(Sport.Football);

        league.Sport.ShouldBe(Sport.Football);
        league.LeagueGender.ShouldBeNull();
    }

    [Fact]
    public void Creates_basketball_league_successfully()
    {
        var league = new League(Sport.Basketball);

        league.Sport.ShouldBe(Sport.Basketball);
        league.LeagueGender.ShouldBeNull();
    }

    [Fact]
    public void Creates_volleyball_male_league_successfully()
    {
        var league = new League(Sport.Volleyball, Gender.Male);

        league.Sport.ShouldBe(Sport.Volleyball);
        league.LeagueGender.ShouldBe(Gender.Male);
    }

    [Fact]
    public void Creates_volleyball_female_league_successfully()
    {
        var league = new League(Sport.Volleyball, Gender.Female);

        league.Sport.ShouldBe(Sport.Volleyball);
        league.LeagueGender.ShouldBe(Gender.Female);
    }

    [Fact]
    public void Fails_when_volleyball_league_has_no_gender()
    {
        Should.Throw<ArgumentException>(() => new League(Sport.Volleyball));
    }

    [Fact]
    public void Non_volleyball_league_ignores_gender()
    {
        var league = new League(Sport.Football, Gender.Male);

        league.LeagueGender.ShouldBeNull();
    }

    // ── StandingEntry entity ──────────────────────────────────────────

    [Fact]
    public void StandingEntry_creates_successfully()
    {
        var entry = new StandingEntry(-1, 1, "Red Lions", "/logos/zvezda.png", 10, 7, 2, 1, 23, 22, 9);

        entry.TeamId.ShouldBe(1);
        entry.TeamName.ShouldBe("Red Lions");
        entry.Points.ShouldBe(23);
        entry.Scored.ShouldBe(22);
        entry.Conceded.ShouldBe(9);
        entry.Difference.ShouldBe(13);
    }

    [Fact]
    public void StandingEntry_difference_is_computed_correctly()
    {
        var entry = new StandingEntry(-1, 1, "Team A", "/logos/zvezda.png", 10, 5, 0, 5, 10, 15, 8);

        entry.Difference.ShouldBe(7);
    }

    [Fact]
    public void StandingEntry_fails_with_empty_team_name()
    {
        Should.Throw<ArgumentException>(() =>
            new StandingEntry(-1, 1, "", "", 10, 7, 2, 1, 23, 20, 9));
    }

    [Fact]
    public void StandingEntry_fails_with_negative_points()
    {
        Should.Throw<ArgumentException>(() =>
            new StandingEntry(-1, 1, "Team A", "", 10, 7, 2, 1, -1, 20, 9));
    }

    [Fact]
    public void StandingEntry_fails_with_negative_scored()
    {
        Should.Throw<ArgumentException>(() =>
            new StandingEntry(-1, 1, "Team A", "", 10, 7, 2, 1, 23, -1, 9));
    }

    [Fact]
    public void StandingEntry_volleyball_has_set_stats()
    {
        var entry = new StandingEntry(-2, 10, "Ace Spikers", "/logos/zvezda.png", 8, 7, 0, 1, 21, 890, 710,
            setWon: 21, setLost: 6);

        entry.SetWon.ShouldBe(21);
        entry.SetLost.ShouldBe(6);
        entry.SetDifference.ShouldBe(15);
    }

    [Fact]
    public void StandingEntry_non_volleyball_has_null_set_stats()
    {
        var entry = new StandingEntry(-1, 1, "Red Lions", "/logos/zvezda.png", 10, 7, 2, 1, 23, 22, 9);

        entry.SetWon.ShouldBeNull();
        entry.SetLost.ShouldBeNull();
        entry.SetDifference.ShouldBeNull();
    }
}