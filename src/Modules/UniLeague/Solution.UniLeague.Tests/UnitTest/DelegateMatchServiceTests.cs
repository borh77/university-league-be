using AutoMapper;
using Moq;
using Shouldly;
using Solution.UniLeague.API.Dtos;
using Solution.UniLeague.Core.Domain;
using Solution.UniLeague.Core.Domain.RepositoryInterfaces;
using Solution.UniLeague.Core.RepositoryInterfaces;
using Solution.UniLeague.Core.UseCases;
using Match = Solution.UniLeague.Core.Domain.Match;

namespace Solution.UniLeague.Tests.UnitTest;

public class DelegateMatchServiceTests
{
    private const long LeagueId = 500;

    [Fact]
    public void SubmitResult_applies_same_playoff_rule_as_admin_when_correcting_a_regular_season_match()
    {
        var regularMatch = CreateMatch(1, MatchStage.RegularSeason, hasResult: true, homeScore: 1, awayScore: 0);

        var leagueRepo = new Mock<ILeagueRepository>();
        leagueRepo.Setup(r => r.GetByIdWithStandings(LeagueId)).Returns(CreateLeague());

        var matchRepo = new Mock<IMatchRepository>();
        matchRepo.Setup(r => r.GetByIdWithResult(regularMatch.Id)).Returns(regularMatch);

        var standingsRecalculation = new Mock<IStandingsRecalculationService>();
        var playoffService = new Mock<IPlayoffService>();
        var delegateService = new DelegateMatchService(
            matchRepo.Object,
            leagueRepo.Object,
            new Mock<ITeamRepository>().Object,
            standingsRecalculation.Object,
            new MatchResultBuilder(new Mock<ITeamRepository>().Object),
            new Mock<IMapper>().Object,
            playoffService.Object);

        delegateService.SubmitResult(regularMatch.Id, new SubmitMatchResultDto { HomeScore = 2, AwayScore = 1 });

        playoffService.Verify(r => r.HandleRegularSeasonResultChanged(LeagueId), Times.Once);
        matchRepo.Verify(r => r.SaveResult(regularMatch), Times.Once);
        standingsRecalculation.Verify(r => r.Recalculate(LeagueId), Times.Once);
    }

    [Fact]
    public void SubmitResult_does_not_apply_the_regular_season_playoff_rule_to_a_playoff_match()
    {
        var playoffMatch = CreateMatch(1, MatchStage.PlayoffSemifinal, hasResult: false);

        var leagueRepo = new Mock<ILeagueRepository>();
        leagueRepo.Setup(r => r.GetByIdWithStandings(LeagueId)).Returns(CreateLeague());

        var matchRepo = new Mock<IMatchRepository>();
        matchRepo.Setup(r => r.GetByIdWithResult(playoffMatch.Id)).Returns(playoffMatch);

        var standingsRecalculation = new Mock<IStandingsRecalculationService>();
        var playoffService = new Mock<IPlayoffService>();
        var delegateService = new DelegateMatchService(
            matchRepo.Object,
            leagueRepo.Object,
            new Mock<ITeamRepository>().Object,
            standingsRecalculation.Object,
            new MatchResultBuilder(new Mock<ITeamRepository>().Object),
            new Mock<IMapper>().Object,
            playoffService.Object);

        delegateService.SubmitResult(playoffMatch.Id, new SubmitMatchResultDto { HomeScore = 3, AwayScore = 1 });

        playoffService.Verify(r => r.HandleRegularSeasonResultChanged(It.IsAny<long>()), Times.Never);
        matchRepo.Verify(r => r.SaveResult(playoffMatch), Times.Once);
        standingsRecalculation.Verify(r => r.Recalculate(LeagueId), Times.Once);
    }

    private static League CreateLeague()
    {
        var league = new League(Sport.Football);
        typeof(League).BaseType!.GetProperty("Id")!.SetValue(league, LeagueId);

        var standings = (List<StandingEntry>)typeof(League)
            .GetField("_standings", System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance)!
            .GetValue(league)!;
        standings.AddRange(new[]
        {
            new StandingEntry(LeagueId, 10, "A", null, 0, 0, 0, 0, 0, 0, 0),
            new StandingEntry(LeagueId, 20, "B", null, 0, 0, 0, 0, 0, 0, 0)
        });

        return league;
    }

    private static Match CreateMatch(
        long id,
        MatchStage stage,
        bool hasResult,
        int homeScore = 0,
        int awayScore = 0)
    {
        var match = stage == MatchStage.RegularSeason
            ? new Match(LeagueId, 1, 10L, "A", "", 20L, "B", "", DateTime.UtcNow)
            : new Match(LeagueId, 3, 10L, "A", "", 20L, "B", "", DateTime.UtcNow, stage, 1, 2);

        typeof(Match).BaseType!.GetProperty("Id")!.SetValue(match, id);

        if (hasResult)
            match.SetResult(MatchResult.Create(homeScore, awayScore));

        return match;
    }
}
