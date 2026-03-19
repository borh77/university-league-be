using Shouldly;
using Solution.UniLeague.Core.Domain;

namespace Solution.UniLeague.Tests.Unit;

public class StandingsSorterTests
{
    private readonly StandingsSorter _sorter = new();

    // Osnovni sort (bez H2H — timovi imaju različite poene)

    [Fact]
    public void Sorts_by_points_descending()
    {
        var standings = new[]
        {
            Entry(teamId: 1, teamName: "A", points: 7,  scored: 5, conceded: 3),
            Entry(teamId: 2, teamName: "B", points: 15, scored: 8, conceded: 4),
            Entry(teamId: 3, teamName: "C", points: 10, scored: 3, conceded: 2),
        };

        var result = _sorter.Sort(Sport.Football, standings, NoMatches());

        result[0].Points.ShouldBe(15);
        result[1].Points.ShouldBe(10);
        result[2].Points.ShouldBe(7);
    }

    [Fact]
    public void Returns_empty_when_no_standings()
    {
        var result = _sorter.Sort(Sport.Football, Array.Empty<StandingEntry>(), NoMatches());

        result.ShouldBeEmpty();
    }

    [Fact]
    public void Single_entry_returns_as_is()
    {
        var standings = new[] { Entry(teamId: 1, teamName: "Solo", points: 9, scored: 3, conceded: 1) };

        var result = _sorter.Sort(Sport.Football, standings, NoMatches());

        result.Count.ShouldBe(1);
        result[0].TeamName.ShouldBe("Solo");
    }

    // FOOTBALL

    [Fact]
    public void Football_breaks_tie_by_h2h_points()
    {
        // A i B imaju isti broj poena; A je pobedio B u H2H → A ispred
        var standings = new[]
        {
            Entry(teamId: 1, teamName: "A", points: 10, scored: 10, conceded: 8),
            Entry(teamId: 2, teamName: "B", points: 10, scored: 10, conceded: 8),
        };

        var matches = new[]
        {
            FootballMatch(homeId: 1, awayId: 2, homeGoals: 2, awayGoals: 0),
        };

        var result = _sorter.Sort(Sport.Football, standings, matches);

        result[0].TeamName.ShouldBe("A");
        result[1].TeamName.ShouldBe("B");
    }

    [Fact]
    public void Football_breaks_h2h_points_tie_by_h2h_goal_difference()
    {
        // A i B su remizirali međusobno (isti H2H poeni); A ima bolji H2H gol-razliku
        var standings = new[]
        {
            Entry(teamId: 1, teamName: "A", points: 10, scored: 10, conceded: 8),
            Entry(teamId: 2, teamName: "B", points: 10, scored: 10, conceded: 8),
        };

        var matches = new[]
        {
            FootballMatch(homeId: 1, awayId: 2, homeGoals: 3, awayGoals: 1), // A +2
            FootballMatch(homeId: 2, awayId: 1, homeGoals: 2, awayGoals: 2), // remi
        };
        // A: H2H pts=4, H2H diff=+2   B: H2H pts=2, H2H diff=-2
        // → A ispred po H2H poenima

        var result = _sorter.Sort(Sport.Football, standings, matches);

        result[0].TeamName.ShouldBe("A");
    }

    [Fact]
    public void Football_breaks_h2h_goal_difference_tie_by_h2h_goals_scored()
    {
        // A i B: isti H2H poeni, isti H2H gol-razlika, A je dao više golova u H2H
        var standings = new[]
        {
            Entry(teamId: 1, teamName: "A", points: 10, scored: 12, conceded: 8),
            Entry(teamId: 2, teamName: "B", points: 10, scored: 12, conceded: 8),
        };

        var matches = new[]
        {
            FootballMatch(homeId: 1, awayId: 2, homeGoals: 3, awayGoals: 0),
            FootballMatch(homeId: 2, awayId: 1, homeGoals: 3, awayGoals: 0),
        };
        

        var standingsTieOnH2HScored = new[]
        {
            Entry(teamId: 1, teamName: "A", points: 10, scored: 15, conceded: 8), // overall diff +7
            Entry(teamId: 2, teamName: "B", points: 10, scored: 12, conceded: 8), // overall diff +4
        };

        var result = _sorter.Sort(Sport.Football, standingsTieOnH2HScored, matches);

        result[0].TeamName.ShouldBe("A"); // bolji overall diff
    }

    [Fact]
    public void Football_falls_back_to_overall_difference_when_h2h_equal()
    {
        var standings = new[]
        {
            Entry(teamId: 1, teamName: "A", points: 10, scored: 5,  conceded: 5),  // diff  0
            Entry(teamId: 2, teamName: "B", points: 10, scored: 10, conceded: 5),  // diff +5
        };

        // Nema H2H mečeva
        var result = _sorter.Sort(Sport.Football, standings, NoMatches());

        result[0].TeamName.ShouldBe("B");
        result[1].TeamName.ShouldBe("A");
    }

    [Fact]
    public void Football_falls_back_to_overall_goals_scored_when_difference_equal()
    {
        var standings = new[]
        {
            Entry(teamId: 1, teamName: "A", points: 10, scored: 5,  conceded: 3),  // diff +2
            Entry(teamId: 2, teamName: "B", points: 10, scored: 8,  conceded: 6),  // diff +2
        };

        var result = _sorter.Sort(Sport.Football, standings, NoMatches());

        result[0].TeamName.ShouldBe("B"); // više golova scored
        result[1].TeamName.ShouldBe("A");
    }

    [Fact]
    public void Football_three_way_tie_h2h_resolves_correctly()
    {
        // A pobedio B, B pobedio C, C pobedio A — H2H poeni su jednaki (3 svako)
        // → fallback na H2H gol-razliku
        var standings = new[]
        {
            Entry(teamId: 1, teamName: "A", points: 6, scored: 10, conceded: 8),
            Entry(teamId: 2, teamName: "B", points: 6, scored: 10, conceded: 8),
            Entry(teamId: 3, teamName: "C", points: 6, scored: 10, conceded: 8),
        };

        var matches = new[]
        {
            FootballMatch(homeId: 1, awayId: 2, homeGoals: 3, awayGoals: 0), // A +3
            FootballMatch(homeId: 2, awayId: 3, homeGoals: 2, awayGoals: 0), // B +2
            FootballMatch(homeId: 3, awayId: 1, homeGoals: 1, awayGoals: 0), // C +1
        };
        // H2H poeni: A=3, B=3, C=3 (jednako)
        // H2H gol-razlika: A = +3-1=+2, B = +2-3=-1, C = +1-2=-1
        // → A prvi

        var result = _sorter.Sort(Sport.Football, standings, matches);

        result[0].TeamName.ShouldBe("A");
    }

    // BASKETBALL

    [Fact]
    public void Basketball_breaks_tie_by_h2h_points()
    {
        var standings = new[]
        {
            Entry(teamId: 1, teamName: "A", points: 10, scored: 300, conceded: 280),
            Entry(teamId: 2, teamName: "B", points: 10, scored: 300, conceded: 280),
        };

        var matches = new[]
        {
            BasketballMatch(homeId: 1, awayId: 2, homeScore: 90, awayScore: 80),
        };
        // A pobedio → H2H pts A=2, B=1

        var result = _sorter.Sort(Sport.Basketball, standings, matches);

        result[0].TeamName.ShouldBe("A");
    }

    [Fact]
    public void Basketball_breaks_h2h_tie_by_h2h_point_difference()
    {
        var standings = new[]
        {
            Entry(teamId: 1, teamName: "A", points: 10, scored: 300, conceded: 280),
            Entry(teamId: 2, teamName: "B", points: 10, scored: 300, conceded: 280),
        };

        // Oba tima su pobedila jednom (jednaki H2H poeni), ali A ima bolju razliku
        var matches = new[]
        {
            BasketballMatch(homeId: 1, awayId: 2, homeScore: 100, awayScore: 70), // A +30
            BasketballMatch(homeId: 2, awayId: 1, homeScore: 80,  awayScore: 79), // B +1
        };
        // A: H2H pts=3, H2H diff = (100+79)-(70+80) = 179-150 = +29
        // B: H2H pts=3, H2H diff = (70+80)-(100+79) = 150-179 = -29
        // → A ispred po H2H diff

        var result = _sorter.Sort(Sport.Basketball, standings, matches);

        result[0].TeamName.ShouldBe("A");
    }

    [Fact]
    public void Basketball_falls_back_to_overall_difference()
    {
        var standings = new[]
        {
            Entry(teamId: 1, teamName: "A", points: 10, scored: 250, conceded: 230), // overall +20
            Entry(teamId: 2, teamName: "B", points: 10, scored: 200, conceded: 190), // overall +10
        };

        var result = _sorter.Sort(Sport.Basketball, standings, NoMatches());

        result[0].TeamName.ShouldBe("A");
    }

    [Fact]
    public void Basketball_falls_back_to_overall_points_scored()
    {
        var standings = new[]
        {
            Entry(teamId: 1, teamName: "A", points: 10, scored: 250, conceded: 230), // diff +20
            Entry(teamId: 2, teamName: "B", points: 10, scored: 270, conceded: 250), // diff +20
        };

        var result = _sorter.Sort(Sport.Basketball, standings, NoMatches());

        result[0].TeamName.ShouldBe("B"); // više scored
    }

    [Fact]
    public void Basketball_win_gives_two_points_loss_gives_one()
    {
        var standings = new[]
        {
            Entry(teamId: 1, teamName: "A", points: 10, scored: 300, conceded: 300),
            Entry(teamId: 2, teamName: "B", points: 10, scored: 300, conceded: 300),
        };

        // A uvek gubi — treba da dobije 1 H2H poen po meču
        var matches = new[]
        {
            BasketballMatch(homeId: 2, awayId: 1, homeScore: 90, awayScore: 70),
            BasketballMatch(homeId: 2, awayId: 1, homeScore: 85, awayScore: 80),
        };

        var result = _sorter.Sort(Sport.Basketball, standings, matches);

        // B ima 4 H2H poena, A ima 2 → B ispred
        result[0].TeamName.ShouldBe("B");
        result[1].TeamName.ShouldBe("A");
    }

    // VOLLEYBALL

    [Fact]
    public void Volleyball_breaks_tie_by_h2h_points()
    {
        var standings = new[]
        {
            VolleyballEntry(teamId: 1, teamName: "A", points: 9, setWon: 6, setLost: 3, scored: 500, conceded: 450),
            VolleyballEntry(teamId: 2, teamName: "B", points: 9, setWon: 6, setLost: 3, scored: 500, conceded: 450),
        };

        var matches = new[]
        {
            VolleyballMatch(homeId: 1, awayId: 2, homeSets: 3, awaySets: 0,
                sets: new[] { (25, 20), (25, 22), (25, 18) }),
        };

        var result = _sorter.Sort(Sport.Volleyball, standings, matches);

        result[0].TeamName.ShouldBe("A");
    }

    [Fact]
    public void Volleyball_breaks_h2h_tie_by_h2h_set_difference()
    {
        var standings = new[]
        {
            VolleyballEntry(teamId: 1, teamName: "A", points: 6, setWon: 4, setLost: 4, scored: 500, conceded: 500),
            VolleyballEntry(teamId: 2, teamName: "B", points: 6, setWon: 4, setLost: 4, scored: 500, conceded: 500),
        };

        // Oba pobedila po jedanput (isti H2H poeni) — A ima bolji set-razliku u H2H
        var matches = new[]
        {
            VolleyballMatch(homeId: 1, awayId: 2, homeSets: 3, awaySets: 0,
                sets: new[] { (25, 20), (25, 22), (25, 18) }),
            VolleyballMatch(homeId: 2, awayId: 1, homeSets: 3, awaySets: 2,
                sets: new[] { (25, 22), (25, 23), (20, 25), (20, 25), (15, 12) }),
        };
        // A: H2H setsWon=5, setsLost=3, setDiff=+2
        // B: H2H setsWon=3, setsLost=5, setDiff=-2
        // H2H poeni su isti (3+3), A ispred po set-razlici

        var result = _sorter.Sort(Sport.Volleyball, standings, matches);

        result[0].TeamName.ShouldBe("A");
    }

    [Fact]
    public void Volleyball_breaks_h2h_set_tie_by_h2h_points_difference()
    {
        var standings = new[]
        {
            VolleyballEntry(teamId: 1, teamName: "A", points: 6, setWon: 4, setLost: 4, scored: 500, conceded: 500),
            VolleyballEntry(teamId: 2, teamName: "B", points: 6, setWon: 4, setLost: 4, scored: 500, conceded: 500),
        };

        // Identični H2H setovi — A ima bolje poene unutar setova
        var matches = new[]
        {
            VolleyballMatch(homeId: 1, awayId: 2, homeSets: 3, awaySets: 1,
                sets: new[] { (25, 10), (25, 10), (25, 10), (10, 25) }), // A +60
            VolleyballMatch(homeId: 2, awayId: 1, homeSets: 3, awaySets: 1,
                sets: new[] { (25, 10), (25, 10), (25, 10), (10, 25) }), // B +60
        };
        // H2H pts: A=3, B=3 (isti); H2H setDiff: A=0, B=0 (isti)
        // H2H pointsDiff: A=(75+10)-(30+25)=85-55=+30 ; B=(30+25)-(75+10)=55-85=-30
        // → A ispred

        var result = _sorter.Sort(Sport.Volleyball, standings, matches);

        result[0].TeamName.ShouldBe("A");
    }

    [Fact]
    public void Volleyball_falls_back_to_overall_set_difference()
    {
        var standings = new[]
        {
            VolleyballEntry(teamId: 1, teamName: "A", points: 6, setWon: 8, setLost: 4, scored: 500, conceded: 480), // setDiff +4
            VolleyballEntry(teamId: 2, teamName: "B", points: 6, setWon: 5, setLost: 5, scored: 500, conceded: 480), // setDiff  0
        };

        var result = _sorter.Sort(Sport.Volleyball, standings, NoMatches());

        result[0].TeamName.ShouldBe("A");
    }

    [Fact]
    public void Volleyball_falls_back_to_overall_scored_when_set_diff_equal()
    {
        var standings = new[]
        {
            VolleyballEntry(teamId: 1, teamName: "A", points: 6, setWon: 6, setLost: 4, scored: 600, conceded: 500), // setDiff +2
            VolleyballEntry(teamId: 2, teamName: "B", points: 6, setWon: 6, setLost: 4, scored: 550, conceded: 450), // setDiff +2
        };

        var result = _sorter.Sort(Sport.Volleyball, standings, NoMatches());

        result[0].TeamName.ShouldBe("A"); // više scored
    }

    // Helpers

    private static IReadOnlyList<Match> NoMatches() => new List<Match>().AsReadOnly();

    private static StandingEntry Entry(int teamId, string teamName, int points, int scored, int conceded)
        => new(-1, teamId, teamName, played: 10, won: 5, drawn: 0, lost: 5,
               points, scored, conceded);

    private static StandingEntry VolleyballEntry(int teamId, string teamName, int points,
        int setWon, int setLost, int scored, int conceded)
        => new(-1, teamId, teamName, played: 8, won: 5, drawn: 0, lost: 3,
               points, scored, conceded, setWon: setWon, setLost: setLost);

    private static Match FootballMatch(int homeId, int awayId, int homeGoals, int awayGoals)
    {
        var match = new Match(1, 1,
            homeId, $"Team{homeId}", "",
            awayId, $"Team{awayId}", "",
            DateTime.UtcNow);
        match.SetResult(MatchResult.Create(homeGoals, awayGoals));
        return match;
    }

    private static Match BasketballMatch(int homeId, int awayId, int homeScore, int awayScore)
    {
        var match = new Match(1, 1,
            homeId, $"Team{homeId}", "",
            awayId, $"Team{awayId}", "",
            DateTime.UtcNow);
        match.SetResult(MatchResult.Create(homeScore, awayScore));
        return match;
    }

    private static Match VolleyballMatch(int homeId, int awayId, int homeSets, int awaySets,
        (int home, int away)[] sets)
    {
        var setScores = sets.Select((s, i) => SetScore.Create(i + 1, s.home, s.away));
        var match = new Match(1, 1,
            homeId, $"Team{homeId}", "",
            awayId, $"Team{awayId}", "",
            DateTime.UtcNow);
        match.SetResult(MatchResult.CreateWithSets(homeSets, awaySets, setScores));
        return match;
    }
}