using Shouldly;
using Solution.UniLeague.Core.Domain;

namespace Solution.UniLeague.Tests.Unit.Domain;

public class LeagueUnitTests
{
    
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

   
    [Fact]
    public void GetSortedStandings_sorts_by_points_descending()
    {
        var league = CreateLeagueWithStandings(new[]
        {
            CreateEntry(leagueId: -1, teamId: 1, teamName: "Team A", points: 10, scored: 5, conceded: 3),
            CreateEntry(leagueId: -1, teamId: 2, teamName: "Team B", points: 15, scored: 8, conceded: 4),
            CreateEntry(leagueId: -1, teamId: 3, teamName: "Team C", points: 7,  scored: 3, conceded: 2),
        });

        var sorted = league.GetSortedStandings();

        sorted[0].Points.ShouldBe(15);
        sorted[1].Points.ShouldBe(10);
        sorted[2].Points.ShouldBe(7);
    }

    [Fact]
    public void GetSortedStandings_breaks_points_tie_by_difference_descending()
    {
        var league = CreateLeagueWithStandings(new[]
        {
            CreateEntry(leagueId: -1, teamId: 1, teamName: "Team A", points: 10, scored: 5,  conceded: 3),  // diff +2
            CreateEntry(leagueId: -1, teamId: 2, teamName: "Team B", points: 10, scored: 10, conceded: 4),  // diff +6
            CreateEntry(leagueId: -1, teamId: 3, teamName: "Team C", points: 10, scored: 3,  conceded: 3),  // diff 0
        });

        var sorted = league.GetSortedStandings();

        sorted[0].TeamName.ShouldBe("Team B"); // diff +6
        sorted[1].TeamName.ShouldBe("Team A"); // diff +2
        sorted[2].TeamName.ShouldBe("Team C"); // diff  0
    }

    [Fact]
    public void GetSortedStandings_breaks_difference_tie_by_scored_descending()
    {
        var league = CreateLeagueWithStandings(new[]
        {
            CreateEntry(leagueId: -1, teamId: 1, teamName: "Team A", points: 10, scored: 5,  conceded: 3),  // diff +2, scored 5
            CreateEntry(leagueId: -1, teamId: 2, teamName: "Team B", points: 10, scored: 8,  conceded: 6),  // diff +2, scored 8
            CreateEntry(leagueId: -1, teamId: 3, teamName: "Team C", points: 10, scored: 3,  conceded: 1),  // diff +2, scored 3
        });

        var sorted = league.GetSortedStandings();

        sorted[0].TeamName.ShouldBe("Team B"); // scored 8
        sorted[1].TeamName.ShouldBe("Team A"); // scored 5
        sorted[2].TeamName.ShouldBe("Team C"); // scored 3
    }

    [Fact]
    public void GetSortedStandings_returns_empty_list_when_no_standings()
    {
        var league = new League(Sport.Football);

        var sorted = league.GetSortedStandings();

        sorted.ShouldBeEmpty();
    }

    
    [Fact]
    public void StandingEntry_creates_successfully()
    {
        var entry = CreateEntry(leagueId: -1, teamId: 1, teamName: "Red Lions", points: 24, scored: 22, conceded: 9);

        entry.TeamId.ShouldBe(1);
        entry.TeamName.ShouldBe("Red Lions");
        entry.Points.ShouldBe(24);
        entry.Scored.ShouldBe(22);
        entry.Conceded.ShouldBe(9);
        entry.Difference.ShouldBe(13);
    }

    [Fact]
    public void StandingEntry_difference_is_computed_correctly()
    {
        var entry = CreateEntry(leagueId: -1, teamId: 1, teamName: "Team A", points: 10, scored: 15, conceded: 8);

        entry.Difference.ShouldBe(7);
    }

    [Fact]
    public void StandingEntry_fails_with_empty_team_name()
    {
        Should.Throw<ArgumentException>(() =>
            new StandingEntry(-1, 1, "", 10, 7, 2, 1, 23, 20, 9));
    }

    [Fact]
    public void StandingEntry_fails_with_negative_points()
    {
        Should.Throw<ArgumentException>(() =>
            new StandingEntry(-1, 1, "Team A", 10, 7, 2, 1, -1, 20, 9));
    }

    [Fact]
    public void StandingEntry_fails_with_negative_scored()
    {
        Should.Throw<ArgumentException>(() =>
            new StandingEntry(-1, 1, "Team A", 10, 7, 2, 1, 23, -1, 9));
    }

    [Fact]
    public void StandingEntry_volleyball_has_set_stats()
    {
        var entry = new StandingEntry(-2, 10, "Ace Spikers", 8, 7, 0, 1, 21, 890, 710, setWon: 21, setLost: 6);

        entry.SetWon.ShouldBe(21);
        entry.SetLost.ShouldBe(6);
        entry.SetDifference.ShouldBe(15);
    }

    [Fact]
    public void StandingEntry_non_volleyball_has_null_set_stats()
    {
        var entry = CreateEntry(leagueId: -1, teamId: 1, teamName: "Red Lions", points: 24, scored: 22, conceded: 9);

        entry.SetWon.ShouldBeNull();
        entry.SetLost.ShouldBeNull();
        entry.SetDifference.ShouldBeNull();
    }

   
    private static League CreateLeagueWithStandings(IEnumerable<StandingEntry> entries)
    {
        var league = new League(Sport.Football);
        var standingsField = typeof(League)
            .GetField("_standings", System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance);
        var list = (List<StandingEntry>)standingsField!.GetValue(league)!;
        list.AddRange(entries);
        return league;
    }

    private static StandingEntry CreateEntry(long leagueId, int teamId, string teamName, int points, int scored, int conceded)
    {
        return new StandingEntry(leagueId, teamId, teamName, played: 10, won: 5, drawn: 0, lost: 5, points, scored, conceded);
    }
}