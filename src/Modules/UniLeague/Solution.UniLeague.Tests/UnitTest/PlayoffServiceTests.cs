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

    [Fact]
    public void Does_not_generate_final_or_third_place_before_both_semifinals_have_results()
    {
        var league = CreateLeagueWithStandings();
        var semifinals = CreateSemifinals(firstHasResult: true, secondHasResult: false);
        var matchRepo = CreateMatchRepo(league.Id, CreateCompletedRegularSeason(), hasPlayoffs: true, playoffMatches: semifinals);
        var service = CreateService(league, matchRepo);

        service.EnsurePlayoffsGenerated(league.Id);

        matchRepo.Verify(
            r => r.AddPlayoffMatchesIfStagesMissing(
                It.IsAny<long>(),
                It.IsAny<IReadOnlyCollection<MatchStage>>(),
                It.IsAny<IReadOnlyCollection<Match>>()),
            Times.Never);
    }

    [Fact]
    public void Generates_final_and_third_place_after_both_semifinals_have_results()
    {
        var league = CreateLeagueWithStandings();
        var captured = new List<Match>();
        var matchRepo = CreateMatchRepo(
            league.Id,
            CreateCompletedRegularSeason(),
            hasPlayoffs: true,
            playoffMatches: CreateSemifinals(firstHasResult: true, secondHasResult: true),
            capturedNextPhaseMatches: captured);
        var service = CreateService(league, matchRepo);

        service.EnsurePlayoffsGenerated(league.Id);

        captured.Count.ShouldBe(2);
        captured.Count(m => m.Stage == MatchStage.PlayoffFinal).ShouldBe(1);
        captured.Count(m => m.Stage == MatchStage.PlayoffThirdPlace).ShouldBe(1);
    }

    [Fact]
    public void Assigns_semifinal_winners_to_final()
    {
        var league = CreateLeagueWithStandings();
        var captured = new List<Match>();
        var matchRepo = CreateMatchRepo(
            league.Id,
            CreateCompletedRegularSeason(),
            hasPlayoffs: true,
            playoffMatches: CreateSemifinals(firstHasResult: true, secondHasResult: true),
            capturedNextPhaseMatches: captured);
        var service = CreateService(league, matchRepo);

        service.EnsurePlayoffsGenerated(league.Id);

        var final = captured.Single(m => m.Stage == MatchStage.PlayoffFinal);
        final.HomeTeamName.ShouldBe("Seed 1");
        final.AwayTeamName.ShouldBe("Seed 3");
        final.HomeSeed.ShouldBe(1);
        final.AwaySeed.ShouldBe(3);
    }

    [Fact]
    public void Assigns_semifinal_losers_to_third_place()
    {
        var league = CreateLeagueWithStandings();
        var captured = new List<Match>();
        var matchRepo = CreateMatchRepo(
            league.Id,
            CreateCompletedRegularSeason(),
            hasPlayoffs: true,
            playoffMatches: CreateSemifinals(firstHasResult: true, secondHasResult: true),
            capturedNextPhaseMatches: captured);
        var service = CreateService(league, matchRepo);

        service.EnsurePlayoffsGenerated(league.Id);

        var thirdPlace = captured.Single(m => m.Stage == MatchStage.PlayoffThirdPlace);
        thirdPlace.HomeTeamName.ShouldBe("Seed 4");
        thirdPlace.AwayTeamName.ShouldBe("Seed 2");
        thirdPlace.HomeSeed.ShouldBe(4);
        thirdPlace.AwaySeed.ShouldBe(2);
    }

    [Fact]
    public void Generates_only_third_place_when_final_already_exists()
    {
        var league = CreateLeagueWithStandings();
        var playoffMatches = CreateSemifinals(firstHasResult: true, secondHasResult: true);
        playoffMatches.Add(CreatePlayoffMatch(12, MatchStage.PlayoffFinal, 1, "Seed 1", 3, "Seed 3", 1, 3));
        var captured = new List<Match>();

        var matchRepo = CreateMatchRepo(
            league.Id,
            CreateCompletedRegularSeason(),
            hasPlayoffs: true,
            playoffMatches: playoffMatches,
            capturedNextPhaseMatches: captured);
        var service = CreateService(league, matchRepo);

        service.EnsurePlayoffsGenerated(league.Id);

        captured.Count.ShouldBe(1);
        captured.Single().Stage.ShouldBe(MatchStage.PlayoffThirdPlace);
    }

    [Fact]
    public void Generates_only_final_when_third_place_already_exists()
    {
        var league = CreateLeagueWithStandings();
        var playoffMatches = CreateSemifinals(firstHasResult: true, secondHasResult: true);
        playoffMatches.Add(CreatePlayoffMatch(13, MatchStage.PlayoffThirdPlace, 4, "Seed 4", 2, "Seed 2", 4, 2));
        var captured = new List<Match>();

        var matchRepo = CreateMatchRepo(
            league.Id,
            CreateCompletedRegularSeason(),
            hasPlayoffs: true,
            playoffMatches: playoffMatches,
            capturedNextPhaseMatches: captured);
        var service = CreateService(league, matchRepo);

        service.EnsurePlayoffsGenerated(league.Id);

        captured.Count.ShouldBe(1);
        captured.Single().Stage.ShouldBe(MatchStage.PlayoffFinal);
    }

    [Fact]
    public void Generates_nothing_when_final_and_third_place_already_exist()
    {
        var league = CreateLeagueWithStandings();
        var playoffMatches = CreateSemifinals(firstHasResult: true, secondHasResult: true);
        playoffMatches.Add(CreatePlayoffMatch(12, MatchStage.PlayoffFinal, 1, "Seed 1", 3, "Seed 3", 1, 3));
        playoffMatches.Add(CreatePlayoffMatch(13, MatchStage.PlayoffThirdPlace, 4, "Seed 4", 2, "Seed 2", 4, 2));

        var matchRepo = CreateMatchRepo(
            league.Id,
            CreateCompletedRegularSeason(),
            hasPlayoffs: true,
            playoffMatches: playoffMatches);
        var service = CreateService(league, matchRepo);

        service.EnsurePlayoffsGenerated(league.Id);

        matchRepo.Verify(
            r => r.AddPlayoffMatchesIfStagesMissing(
                It.IsAny<long>(),
                It.IsAny<IReadOnlyCollection<MatchStage>>(),
                It.IsAny<IReadOnlyCollection<Match>>()),
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
        List<Match>? capturedMatches = null,
        List<Match>? playoffMatches = null,
        List<Match>? capturedNextPhaseMatches = null)
    {
        var matchRepo = new Mock<IMatchRepository>();
        matchRepo.Setup(r => r.HasPlayoffSemifinals(leagueId)).Returns(hasPlayoffs);
        matchRepo.Setup(r => r.GetRegularSeasonMatchesByLeague(leagueId)).Returns(regularMatches);
        matchRepo.Setup(r => r.GetPlayoffMatchesByLeague(leagueId)).Returns(playoffMatches ?? new List<Match>());
        matchRepo
            .Setup(r => r.AddPlayoffSemifinalsIfNone(leagueId, It.IsAny<IReadOnlyCollection<Match>>()))
            .Callback<long, IReadOnlyCollection<Match>>((_, matches) =>
            {
                capturedMatches?.AddRange(matches);
            })
            .Returns(true);
        matchRepo
            .Setup(r => r.AddPlayoffMatchesIfStagesMissing(
                leagueId,
                It.IsAny<IReadOnlyCollection<MatchStage>>(),
                It.IsAny<IReadOnlyCollection<Match>>()))
            .Callback<long, IReadOnlyCollection<MatchStage>, IReadOnlyCollection<Match>>((_, _, matches) =>
            {
                capturedNextPhaseMatches?.AddRange(matches);
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

    private static List<Match> CreateSemifinals(bool firstHasResult, bool secondHasResult)
    {
        var first = CreatePlayoffMatch(
            10,
            MatchStage.PlayoffSemifinal,
            1,
            "Seed 1",
            4,
            "Seed 4",
            1,
            4);
        var second = CreatePlayoffMatch(
            11,
            MatchStage.PlayoffSemifinal,
            2,
            "Seed 2",
            3,
            "Seed 3",
            2,
            3);

        if (firstHasResult)
            first.SetResult(MatchResult.Create(2, 0));

        if (secondHasResult)
            second.SetResult(MatchResult.Create(0, 1));

        return new List<Match> { first, second };
    }

    private static Match CreatePlayoffMatch(
        long id,
        MatchStage stage,
        long homeTeamId,
        string homeTeamName,
        long awayTeamId,
        string awayTeamName,
        int homeSeed,
        int awaySeed)
    {
        var match = new Match(
            100,
            stage == MatchStage.PlayoffSemifinal ? 3 : 4,
            homeTeamId,
            homeTeamName,
            "/logo.png",
            awayTeamId,
            awayTeamName,
            "/logo.png",
            new DateTime(2026, 1, 10 + (int)id, 18, 0, 0, DateTimeKind.Utc),
            stage,
            homeSeed,
            awaySeed);

        typeof(Match).BaseType!.GetProperty("Id")!.SetValue(match, id);

        return match;
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
