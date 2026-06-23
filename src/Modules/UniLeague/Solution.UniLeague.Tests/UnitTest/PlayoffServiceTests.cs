using Moq;
using Shouldly;
using Solution.UniLeague.Core.Domain;
using Solution.UniLeague.Core.Domain.RepositoryInterfaces;
using Solution.UniLeague.Core.RepositoryInterfaces;
using Solution.UniLeague.Core.UseCases;
using Xunit;
using Match = Solution.UniLeague.Core.Domain.Match;

namespace Solution.UniLeague.Tests.UnitTest;

public class PlayoffServiceTests
{
    [Fact]
    public void Does_not_generate_playoffs_before_all_regular_season_matches_have_results()
    {
        var league = CreateLeagueWithStandings();
        var regularMatches = CreateCompletedRegularSeason();
        regularMatches[0] = CreateMatch(1, league.Id, 1, 1, "Seed 1", 2, "Seed 2");

        var matchRepo = CreateMatchRepo(league.Id, regularMatches);
        var service = CreateService(league, matchRepo);

        service.EnsurePlayoffsGenerated(league.Id);

        matchRepo.Verify(
            r => r.AddPlayoffSemifinalsIfNone(It.IsAny<long>(), It.IsAny<IReadOnlyCollection<Match>>()),
            Times.Never);
    }

    [Fact]
    public void Generates_exactly_two_playoff_semifinal_matches_after_all_regular_season_matches_have_results()
    {
        var league = CreateLeagueWithStandings();
        var matchRepo = CreateMatchRepo(league.Id, CreateCompletedRegularSeason());
        var service = CreateService(league, matchRepo);

        service.EnsurePlayoffsGenerated(league.Id);

        matchRepo.Verify(
            r => r.AddPlayoffSemifinalsIfNone(
                league.Id,
                It.Is<IReadOnlyCollection<Match>>(matches =>
                    matches.Count == 2 &&
                    matches.All(m => m.Stage == MatchStage.PlayoffSemifinal))),
            Times.Once);
    }

    [Fact]
    public void Does_not_duplicate_playoff_matches_when_semifinals_already_exist()
    {
        var league = CreateLeagueWithStandings();
        var matchRepo = CreateMatchRepo(league.Id, CreateCompletedRegularSeason(), hasPlayoffs: true);
        var service = CreateService(league, matchRepo);

        service.EnsurePlayoffsGenerated(league.Id);

        matchRepo.Verify(r => r.GetRegularSeasonMatchesByLeague(It.IsAny<long>()), Times.Never);
        matchRepo.Verify(
            r => r.AddPlayoffSemifinalsIfNone(It.IsAny<long>(), It.IsAny<IReadOnlyCollection<Match>>()),
            Times.Never);
    }

    [Fact]
    public void Uses_correct_semifinal_seeds()
    {
        var league = CreateLeagueWithStandings();
        var captured = new List<Match>();
        var matchRepo = CreateMatchRepo(league.Id, CreateCompletedRegularSeason(), capturedMatches: captured);
        var service = CreateService(league, matchRepo);

        service.EnsurePlayoffsGenerated(league.Id);

        captured.Count.ShouldBe(2);
        captured[0].HomeSeed.ShouldBe(1);
        captured[0].AwaySeed.ShouldBe(4);
        captured[0].HomeTeamName.ShouldBe("Seed 1");
        captured[0].AwayTeamName.ShouldBe("Seed 4");
        captured[1].HomeSeed.ShouldBe(2);
        captured[1].AwaySeed.ShouldBe(3);
        captured[1].HomeTeamName.ShouldBe("Seed 2");
        captured[1].AwayTeamName.ShouldBe("Seed 3");
    }

    [Fact]
    public void Does_not_generate_when_fewer_than_four_teams_exist_in_standings()
    {
        var league = CreateLeagueWithStandings(teamCount: 3);
        var matchRepo = CreateMatchRepo(league.Id, CreateCompletedRegularSeason());
        var service = CreateService(league, matchRepo);

        service.EnsurePlayoffsGenerated(league.Id);

        matchRepo.Verify(
            r => r.AddPlayoffSemifinalsIfNone(It.IsAny<long>(), It.IsAny<IReadOnlyCollection<Match>>()),
            Times.Never);
    }

    private static PlayoffService CreateService(League league, Mock<IMatchRepository> matchRepo)
    {
        var leagueRepo = new Mock<ILeagueRepository>();
        leagueRepo.Setup(r => r.GetByIdWithStandings(league.Id)).Returns(league);

        return new PlayoffService(leagueRepo.Object, matchRepo.Object);
    }

    private static Mock<IMatchRepository> CreateMatchRepo(
        long leagueId,
        List<Match> regularMatches,
        bool hasPlayoffs = false,
        List<Match>? capturedMatches = null)
    {
        var matchRepo = new Mock<IMatchRepository>();
        matchRepo.Setup(r => r.HasPlayoffSemifinals(leagueId)).Returns(hasPlayoffs);
        matchRepo.Setup(r => r.GetRegularSeasonMatchesByLeague(leagueId)).Returns(regularMatches);
        matchRepo
            .Setup(r => r.AddPlayoffSemifinalsIfNone(leagueId, It.IsAny<IReadOnlyCollection<Match>>()))
            .Callback<long, IReadOnlyCollection<Match>>((_, matches) =>
            {
                capturedMatches?.AddRange(matches);
            })
            .Returns(true);

        return matchRepo;
    }

    private static League CreateLeagueWithStandings(int teamCount = 4)
    {
        var league = new League(Sport.Football);
        typeof(League).BaseType!.GetProperty("Id")!.SetValue(league, 100L);

        var standings = GetStandingsField(league);
        for (var i = 1; i <= teamCount; i++)
        {
            standings.Add(new StandingEntry(
                league.Id,
                i,
                $"Seed {i}",
                $"/logos/{i}.png",
                3,
                4 - i,
                0,
                i - 1,
                (4 - i) * 3,
                10 - i,
                i));
        }

        return league;
    }

    private static List<Match> CreateCompletedRegularSeason()
    {
        return new List<Match>
        {
            CreateMatch(1, 100, 1, 1, "Seed 1", 2, "Seed 2", hasResult: true),
            CreateMatch(2, 100, 1, 3, "Seed 3", 4, "Seed 4", hasResult: true),
            CreateMatch(3, 100, 2, 1, "Seed 1", 3, "Seed 3", hasResult: true),
            CreateMatch(4, 100, 2, 2, "Seed 2", 4, "Seed 4", hasResult: true)
        };
    }

    private static Match CreateMatch(
        long id,
        long leagueId,
        int roundNumber,
        long homeTeamId,
        string homeTeamName,
        long awayTeamId,
        string awayTeamName,
        bool hasResult = false)
    {
        var match = new Match(
            leagueId,
            roundNumber,
            homeTeamId,
            homeTeamName,
            "/logo.png",
            awayTeamId,
            awayTeamName,
            "/logo.png",
            new DateTime(2026, 1, roundNumber, 18, 0, 0, DateTimeKind.Utc));

        typeof(Match).BaseType!.GetProperty("Id")!.SetValue(match, id);

        if (hasResult)
            match.SetResult(MatchResult.Create(1, 0));

        return match;
    }

    private static List<StandingEntry> GetStandingsField(League league)
    {
        var field = typeof(League).GetField(
            "_standings",
            System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance);

        return (List<StandingEntry>)field!.GetValue(league)!;
    }
}
