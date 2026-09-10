using Moq;
using Shouldly;
using Solution.BuildingBlocks.Core.Exceptions;
using Solution.UniLeague.Core.Domain;
using Solution.UniLeague.Core.Domain.RepositoryInterfaces;
using Solution.UniLeague.Core.RepositoryInterfaces;
using Solution.UniLeague.Core.UseCases;
using Match = Solution.UniLeague.Core.Domain.Match;

namespace Solution.UniLeague.Tests.Unit;

public class StandingsRecalculationServiceTests
{
    private const long LeagueId = -1;

    [Fact]
    public void Builds_football_table_from_played_matches()
    {
        var matches = new[]
        {
            FootballMatch(1, "A", 2, "B", 2, 1),
            FootballMatch(3, "C", 1, "A", 0, 0),
        };

        var captured = Recalculate(Sport.Football, matches);

        captured.Count.ShouldBe(3);

        var a = captured.Single(e => e.TeamName == "A");
        a.Played.ShouldBe(2);
        a.Won.ShouldBe(1);
        a.Drawn.ShouldBe(1);
        a.Lost.ShouldBe(0);
        a.Points.ShouldBe(4);
        a.Scored.ShouldBe(2);
        a.Conceded.ShouldBe(1);

        var b = captured.Single(e => e.TeamName == "B");
        b.Points.ShouldBe(0);
        b.Lost.ShouldBe(1);

        var c = captured.Single(e => e.TeamName == "C");
        c.Points.ShouldBe(1);
        c.Drawn.ShouldBe(1);
    }

    [Fact]
    public void Basketball_gives_two_points_for_win_and_one_for_loss()
    {
        var matches = new[] { BasketballMatch(1, "A", 2, "B", 90, 80) };

        var captured = Recalculate(Sport.Basketball, matches);

        captured.Single(e => e.TeamName == "A").Points.ShouldBe(2);
        captured.Single(e => e.TeamName == "B").Points.ShouldBe(1);
        captured.ShouldAllBe(e => e.SetWon == null);
    }

    [Fact]
    public void Volleyball_gives_three_points_for_win_and_fills_set_columns()
    {
        var matches = new[]
        {
            VolleyballMatch(1, "A", 2, "B", 3, 1,
                new[] { (25, 20), (25, 22), (20, 25), (25, 18) }),
        };

        var captured = Recalculate(Sport.Volleyball, matches);

        var a = captured.Single(e => e.TeamName == "A");
        a.Points.ShouldBe(3);
        a.Won.ShouldBe(1);
        a.SetWon.ShouldBe(3);
        a.SetLost.ShouldBe(1);
        a.Scored.ShouldBe(95);
        a.Conceded.ShouldBe(85);

        var b = captured.Single(e => e.TeamName == "B");
        b.Points.ShouldBe(0);
        b.Lost.ShouldBe(1);
        b.SetWon.ShouldBe(1);
        b.SetLost.ShouldBe(3);
    }

    [Fact]
    public void Two_consecutive_runs_produce_the_same_table()
    {
        var matches = new[]
        {
            FootballMatch(1, "A", 2, "B", 2, 1),
            FootballMatch(2, "B", 3, "C", 1, 1),
            FootballMatch(3, "C", 1, "A", 0, 3),
        };

        var (leagueRepo, matchRepo) = Repos(Sport.Football, matches);
        var standingsRepo = new Mock<IStandingsRepository>();
        var runs = new List<IReadOnlyCollection<StandingEntry>>();
        standingsRepo
            .Setup(r => r.ReplaceForLeague(LeagueId, It.IsAny<IReadOnlyCollection<StandingEntry>>()))
            .Callback<long, IReadOnlyCollection<StandingEntry>>((_, entries) => runs.Add(entries));

        var service = new StandingsRecalculationService(
            leagueRepo.Object, matchRepo.Object, standingsRepo.Object);

        service.Recalculate(LeagueId);
        service.Recalculate(LeagueId);

        runs.Count.ShouldBe(2);
        Serialize(runs[0]).ShouldBe(Serialize(runs[1]));
    }

    [Fact]
    public void Keeps_a_row_for_every_league_team_even_without_a_played_match()
    {
        var league = LeagueWith(Sport.Football,
            Entry(1, "Alpha"), Entry(2, "Beta"), Entry(3, "Gamma"), Entry(4, "Delta"));

        // odigran samo Alpha - Beta
        var matches = new[] { FootballMatch(1, "Alpha", 2, "Beta", 2, 0) };

        var captured = Recalculate(league, matches);

        captured.Count.ShouldBe(4);
        captured.Count(e => e.Played == 0).ShouldBe(2);

        captured.Single(e => e.TeamName == "Alpha").Points.ShouldBe(3);
        captured.Single(e => e.TeamName == "Beta").Played.ShouldBe(1);

        var gamma = captured.Single(e => e.TeamName == "Gamma");
        gamma.Played.ShouldBe(0);
        gamma.Won.ShouldBe(0);
        gamma.Points.ShouldBe(0);
        gamma.Scored.ShouldBe(0);
    }

    [Fact]
    public void Carries_team_name_and_logo_for_teams_without_a_played_match()
    {
        var league = LeagueWith(Sport.Football,
            Entry(1, "Alpha", "/logos/alpha.png"),
            Entry(2, "Beta", "/logos/beta.png"),
            Entry(3, "Gamma", "/logos/gamma.png"),
            Entry(4, "Delta", "/logos/delta.png"));

        var matches = new[] { FootballMatch(1, "Alpha", 2, "Beta", 1, 1) };

        var captured = Recalculate(league, matches);

        var gamma = captured.Single(e => e.TeamId == 3);
        gamma.TeamName.ShouldBe("Gamma");
        gamma.LogoUrl.ShouldBe("/logos/gamma.png");

        var delta = captured.Single(e => e.TeamId == 4);
        delta.TeamName.ShouldBe("Delta");
        delta.LogoUrl.ShouldBe("/logos/delta.png");
    }

    [Fact]
    public void Throws_not_found_when_league_missing()
    {
        var leagueRepo = new Mock<ILeagueRepository>();
        leagueRepo.Setup(r => r.GetByIdWithStandings(It.IsAny<long>())).Returns((League?)null);

        var service = new StandingsRecalculationService(
            leagueRepo.Object,
            new Mock<IMatchRepository>().Object,
            new Mock<IStandingsRepository>().Object);

        Should.Throw<NotFoundException>(() => service.Recalculate(LeagueId));
    }

    // ── Helpers ──────────────────────────────────────────────────────

    private static List<StandingEntry> Recalculate(Sport sport, IReadOnlyList<Match> matches)
    {
        var league = sport == Sport.Volleyball
            ? new League(Sport.Volleyball, Gender.Male)
            : new League(sport);
        return Recalculate(league, matches);
    }

    private static List<StandingEntry> Recalculate(League league, IReadOnlyList<Match> matches)
    {
        var (leagueRepo, matchRepo) = Repos(league, matches);
        var standingsRepo = new Mock<IStandingsRepository>();
        IReadOnlyCollection<StandingEntry> captured = Array.Empty<StandingEntry>();
        standingsRepo
            .Setup(r => r.ReplaceForLeague(LeagueId, It.IsAny<IReadOnlyCollection<StandingEntry>>()))
            .Callback<long, IReadOnlyCollection<StandingEntry>>((_, entries) => captured = entries);

        new StandingsRecalculationService(leagueRepo.Object, matchRepo.Object, standingsRepo.Object)
            .Recalculate(LeagueId);

        return captured.ToList();
    }

    private static (Mock<ILeagueRepository>, Mock<IMatchRepository>) Repos(
        Sport sport, IReadOnlyList<Match> matches)
    {
        var league = sport == Sport.Volleyball
            ? new League(Sport.Volleyball, Gender.Male)
            : new League(sport);
        return Repos(league, matches);
    }

    private static (Mock<ILeagueRepository>, Mock<IMatchRepository>) Repos(
        League league, IReadOnlyList<Match> matches)
    {
        var leagueRepo = new Mock<ILeagueRepository>();
        leagueRepo.Setup(r => r.GetByIdWithStandings(LeagueId)).Returns(league);

        var matchRepo = new Mock<IMatchRepository>();
        matchRepo.Setup(r => r.GetAllPlayedByLeague(LeagueId)).Returns(matches.ToList());

        return (leagueRepo, matchRepo);
    }

    private static League LeagueWith(Sport sport, params StandingEntry[] entries)
    {
        var league = sport == Sport.Volleyball
            ? new League(Sport.Volleyball, Gender.Male)
            : new League(sport);

        var field = typeof(League).GetField("_standings",
            System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance);
        ((List<StandingEntry>)field!.GetValue(league)!).AddRange(entries);

        return league;
    }

    private static StandingEntry Entry(int teamId, string teamName, string? logoUrl = null)
        => new(LeagueId, teamId, teamName, logoUrl, 0, 0, 0, 0, 0, 0, 0);

    private static string Serialize(IReadOnlyCollection<StandingEntry> entries)
        => string.Join("\n", entries
            .OrderBy(e => e.TeamId)
            .Select(e => $"{e.TeamId}|{e.TeamName}|P{e.Played}|W{e.Won}|D{e.Drawn}|L{e.Lost}" +
                         $"|Pts{e.Points}|GF{e.Scored}|GA{e.Conceded}|SW{e.SetWon}|SL{e.SetLost}"));

    private static Match FootballMatch(long homeId, string home, long awayId, string away, int hs, int @as)
    {
        var match = new Match(LeagueId, 1, homeId, home, "", awayId, away, "", DateTime.UtcNow);
        match.SetResult(MatchResult.Create(hs, @as));
        return match;
    }

    private static Match BasketballMatch(long homeId, string home, long awayId, string away, int hs, int @as)
    {
        var match = new Match(LeagueId, 1, homeId, home, "", awayId, away, "", DateTime.UtcNow);
        match.SetResult(MatchResult.Create(hs, @as));
        return match;
    }

    private static Match VolleyballMatch(
        long homeId, string home, long awayId, string away,
        int homeSets, int awaySets, (int home, int away)[] sets)
    {
        var match = new Match(LeagueId, 1, homeId, home, "", awayId, away, "", DateTime.UtcNow);
        var setScores = sets.Select((s, i) => SetScore.Create(i + 1, s.home, s.away));
        match.SetResult(MatchResult.CreateWithSets(homeSets, awaySets, setScores));
        return match;
    }
}
