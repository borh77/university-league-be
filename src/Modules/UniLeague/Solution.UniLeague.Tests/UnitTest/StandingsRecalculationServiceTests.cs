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
        var (leagueRepo, matchRepo) = Repos(sport, matches);
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

        var leagueRepo = new Mock<ILeagueRepository>();
        leagueRepo.Setup(r => r.GetByIdWithStandings(LeagueId)).Returns(league);

        var matchRepo = new Mock<IMatchRepository>();
        matchRepo.Setup(r => r.GetAllPlayedByLeague(LeagueId)).Returns(matches.ToList());

        return (leagueRepo, matchRepo);
    }

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
