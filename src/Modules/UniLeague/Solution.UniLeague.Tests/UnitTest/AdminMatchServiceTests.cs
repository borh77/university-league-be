using Moq;
using Shouldly;
using Solution.UniLeague.API.Dtos;
using Solution.UniLeague.Core.Domain;
using Solution.UniLeague.Core.Domain.RepositoryInterfaces;
using Solution.UniLeague.Core.RepositoryInterfaces;
using Solution.UniLeague.Core.UseCases;
using Match = Solution.UniLeague.Core.Domain.Match;

namespace Solution.UniLeague.Tests.UnitTest;

public class AdminMatchServiceTests
{
    private const long LeagueId = 500;

    [Fact]
    public void ClearResult_and_recalculate_returns_table_to_state_before_the_cleared_match()
    {
        var league = CreateLeague();

        var alphaVsBeta = CreateMatch(1, 10, "A", 20, "B", hasResult: true, homeScore: 2, awayScore: 1);
        var betaVsGamma = CreateMatch(2, 20, "B", 30, "C", hasResult: true, homeScore: 1, awayScore: 1);
        var allMatches = new List<Match> { alphaVsBeta, betaVsGamma };

        var leagueRepo = new Mock<ILeagueRepository>();
        leagueRepo.Setup(r => r.GetByIdWithStandings(LeagueId)).Returns(league);

        var matchRepo = new Mock<IMatchRepository>();
        matchRepo.Setup(r => r.GetByIdWithResult(betaVsGamma.Id)).Returns(betaVsGamma);
        matchRepo.Setup(r => r.GetPlayoffMatchesByLeague(LeagueId)).Returns(new List<Match>());
        // Odigrani mecevi se cita iznova posle ClearResult-a - betaVsGamma vise nema rezultat
        matchRepo.Setup(r => r.GetAllPlayedByLeague(LeagueId))
            .Returns(() => allMatches.Where(m => m.HasResult).ToList());

        var runs = new List<List<StandingEntry>>();
        var standingsRepo = new Mock<IStandingsRepository>();
        standingsRepo
            .Setup(r => r.ReplaceForLeague(LeagueId, It.IsAny<IReadOnlyCollection<StandingEntry>>()))
            .Callback<long, IReadOnlyCollection<StandingEntry>>((_, entries) => runs.Add(entries.ToList()));

        var recalculationService = new StandingsRecalculationService(leagueRepo.Object, matchRepo.Object, standingsRepo.Object);
        var adminService = new AdminMatchService(
            matchRepo.Object,
            leagueRepo.Object,
            new Mock<ITeamRepository>().Object,
            recalculationService,
            new MatchResultBuilder(new Mock<ITeamRepository>().Object),
            new Mock<IPlayoffService>().Object);

        adminService.ClearResult(betaVsGamma.Id);

        var finalStandings = runs.Last();

        var beta = finalStandings.Single(e => e.TeamName == "B");
        beta.Played.ShouldBe(1);
        beta.Points.ShouldBe(0);

        var gamma = finalStandings.Single(e => e.TeamName == "C");
        gamma.Played.ShouldBe(0);
        gamma.Points.ShouldBe(0);
    }

    [Fact]
    public void UpdateMatch_rejects_partial_team_change()
    {
        var match = CreateMatch(1, 10, "A", 20, "B");

        var leagueRepo = new Mock<ILeagueRepository>();
        var matchRepo = new Mock<IMatchRepository>();
        matchRepo.Setup(r => r.GetByIdWithResult(match.Id)).Returns(match);

        var adminService = new AdminMatchService(
            matchRepo.Object,
            leagueRepo.Object,
            new Mock<ITeamRepository>().Object,
            new Mock<IStandingsRecalculationService>().Object,
            new MatchResultBuilder(new Mock<ITeamRepository>().Object),
            new Mock<IPlayoffService>().Object);

        Should.Throw<ArgumentException>(() =>
            adminService.UpdateMatch(match.Id, new UpdateMatchDto { HomeTeamId = 99 }));

        match.HomeTeamId.ShouldBe(10L);
        matchRepo.Verify(r => r.Save(It.IsAny<Match>()), Times.Never);
    }

    [Fact]
    public void UpdateResult_allows_regular_season_edit_and_leaves_playoff_bracket_untouched_when_playoff_already_has_results()
    {
        var regularMatch = CreateMatch(1, 10, "A", 20, "B");

        var leagueRepo = new Mock<ILeagueRepository>();
        leagueRepo.Setup(r => r.GetByIdWithStandings(LeagueId)).Returns(CreateLeague());

        var matchRepo = new Mock<IMatchRepository>();
        matchRepo.Setup(r => r.GetByIdWithResult(regularMatch.Id)).Returns(regularMatch);

        var standingsRecalculation = new Mock<IStandingsRecalculationService>();
        var playoffService = new Mock<IPlayoffService>();
        var adminService = new AdminMatchService(
            matchRepo.Object,
            leagueRepo.Object,
            new Mock<ITeamRepository>().Object,
            standingsRecalculation.Object,
            new MatchResultBuilder(new Mock<ITeamRepository>().Object),
            playoffService.Object);

        adminService.UpdateResult(regularMatch.Id, new SubmitMatchResultDto { HomeScore = 1, AwayScore = 0 });

        playoffService.Verify(r => r.HandleRegularSeasonResultChanged(LeagueId), Times.Once);
        matchRepo.Verify(r => r.Save(regularMatch), Times.Once);
        standingsRecalculation.Verify(r => r.Recalculate(LeagueId), Times.Once);
    }

    [Fact]
    public void UpdateResult_rejects_invalid_dto_without_touching_playoff_bracket()
    {
        var regularMatch = CreateMatch(1, 10, "A", 20, "B");

        var leagueRepo = new Mock<ILeagueRepository>();
        leagueRepo.Setup(r => r.GetByIdWithStandings(LeagueId)).Returns(CreateLeague());

        var matchRepo = new Mock<IMatchRepository>();
        matchRepo.Setup(r => r.GetByIdWithResult(regularMatch.Id)).Returns(regularMatch);

        var standingsRecalculation = new Mock<IStandingsRecalculationService>();
        var playoffService = new Mock<IPlayoffService>();
        var adminService = new AdminMatchService(
            matchRepo.Object,
            leagueRepo.Object,
            new Mock<ITeamRepository>().Object,
            standingsRecalculation.Object,
            new MatchResultBuilder(new Mock<ITeamRepository>().Object),
            playoffService.Object);

        // Golovi ne odgovaraju unetom rezultatu - MatchResultBuilder mora da pukne pre nego sto se dirne plej-of
        var invalidRequest = new SubmitMatchResultDto
        {
            HomeScore = 2,
            AwayScore = 1,
            Goals = new List<GoalEventDto>
            {
                new() { ScorerName = "Igrac", TeamName = "A", IsHomeTeamGoal = true, Minute = 10 }
            }
        };

        Should.Throw<ArgumentException>(() => adminService.UpdateResult(regularMatch.Id, invalidRequest));

        playoffService.Verify(r => r.HandleRegularSeasonResultChanged(It.IsAny<long>()), Times.Never);
        matchRepo.Verify(r => r.Save(It.IsAny<Match>()), Times.Never);
        standingsRecalculation.Verify(r => r.Recalculate(It.IsAny<long>()), Times.Never);
        regularMatch.HasResult.ShouldBeFalse();
    }

    private static League CreateLeague()
    {
        var league = new League(Sport.Football);
        typeof(League).BaseType!.GetProperty("Id")!.SetValue(league, LeagueId);

        // Standings vec sadrzi sva tri tima - isto sto AdminLeagueService.AddTeamToLeague radi
        var standings = (List<StandingEntry>)typeof(League)
            .GetField("_standings", System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance)!
            .GetValue(league)!;
        standings.AddRange(new[]
        {
            new StandingEntry(LeagueId, 10, "A", null, 0, 0, 0, 0, 0, 0, 0),
            new StandingEntry(LeagueId, 20, "B", null, 0, 0, 0, 0, 0, 0, 0),
            new StandingEntry(LeagueId, 30, "C", null, 0, 0, 0, 0, 0, 0, 0)
        });

        return league;
    }

    private static Match CreateMatch(
        long id,
        long homeTeamId, string homeTeamName,
        long awayTeamId, string awayTeamName,
        bool hasResult = false,
        int homeScore = 0,
        int awayScore = 0)
    {
        var match = new Match(
            LeagueId, 1,
            homeTeamId, homeTeamName, "",
            awayTeamId, awayTeamName, "",
            DateTime.UtcNow);

        typeof(Match).BaseType!.GetProperty("Id")!.SetValue(match, id);

        if (hasResult)
            match.SetResult(MatchResult.Create(homeScore, awayScore));

        return match;
    }
}
