using AutoMapper;
using Moq;
using Shouldly;
using Solution.BuildingBlocks.Core.UseCases;
using Solution.UniLeague.API.Dtos;
using Solution.UniLeague.Core.Domain;
using Solution.UniLeague.Core.Domain.RepositoryInterfaces;
using Solution.UniLeague.Core.Mappers;
using Solution.UniLeague.Core.UseCases;
using Xunit;

namespace Solution.UniLeague.Tests.UnitTest;

public class LeagueServiceTests
{
    [Fact]
    public void Returns_empty_list_when_no_matches_found()
    {
        var repo = new Mock<IMatchRepository>();
        repo.Setup(r => r.GetScheduleByLeague(It.IsAny<long>()))
            .Returns(new PagedResult<Solution.UniLeague.Core.Domain.Match>(new List<Solution.UniLeague.Core.Domain.Match>(), 0));

        var result = CreateService(repo).GetScheduleByLeague(999L);

        result.ShouldNotBeNull();
        result.Count.ShouldBe(0);
    }

    [Fact]
    public void Returns_matches_for_existing_league()
    {
        var leagueId = 1L;
        var matches = new List<Solution.UniLeague.Core.Domain.Match>
        {
            CreateMatch(1, leagueId, 1, "Crvena zvezda", "Partizan"),
            CreateMatch(2, leagueId, 1, "Vojvodina", "Čukarički"),
            CreateMatch(3, leagueId, 2, "Partizan", "Vojvodina"),
            CreateMatch(4, leagueId, 2, "Čukarički", "Crvena zvezda")
        };

        var repo = new Mock<IMatchRepository>();
        repo.Setup(r => r.GetScheduleByLeague(leagueId))
            .Returns(new PagedResult<Solution.UniLeague.Core.Domain.Match>(matches, matches.Count));

        var result = CreateService(repo).GetScheduleByLeague(leagueId);

        result.ShouldNotBeNull();
        result.Count.ShouldBe(4);
        result.All(m => m.LeagueId == leagueId).ShouldBeTrue();
        result[0].HomeTeamName.ShouldBe("Crvena zvezda");
        result[0].AwayTeamName.ShouldBe("Partizan");
    }

    [Fact]
    public void Maps_all_match_properties_correctly()
    {
        var leagueId = 1L;
        var scheduledAt = new DateTime(2025, 9, 14, 18, 0, 0);
        var match = CreateMatchDetailed(
            100, leagueId, 3,
            20, "Crvena zvezda", "/logos/zvezda.png",
            21, "Partizan", "/logos/partizan.png",
            scheduledAt);

        var repo = new Mock<IMatchRepository>();
        repo.Setup(r => r.GetScheduleByLeague(leagueId))
            .Returns(new PagedResult<Solution.UniLeague.Core.Domain.Match>(new List<Solution.UniLeague.Core.Domain.Match> { match }, 1));

        var result = CreateService(repo).GetScheduleByLeague(leagueId);

        result.Count.ShouldBe(1);
        var dto = result[0];
        dto.Id.ShouldBe(100);
        dto.LeagueId.ShouldBe(leagueId);
        dto.RoundNumber.ShouldBe(3);
        dto.HomeTeamId.ShouldBe(20);
        dto.HomeTeamName.ShouldBe("Crvena zvezda");
        dto.HomeTeamLogoUrl.ShouldBe("/logos/zvezda.png");
        dto.AwayTeamId.ShouldBe(21);
        dto.AwayTeamName.ShouldBe("Partizan");
        dto.AwayTeamLogoUrl.ShouldBe("/logos/partizan.png");
        dto.ScheduledAt.ShouldBe(scheduledAt);
    }

    [Fact]
    public void Maps_playoff_metadata_for_schedule_matches()
    {
        var leagueId = 1L;
        var semifinal = new Solution.UniLeague.Core.Domain.Match(
            leagueId, 3,
            1, "Seed 1", "/logos/1.png",
            4, "Seed 4", "/logos/4.png",
            new DateTime(2026, 6, 1, 18, 0, 0),
            MatchStage.PlayoffSemifinal,
            1,
            4);
        var final = new Solution.UniLeague.Core.Domain.Match(
            leagueId, 4,
            1, "Seed 1", "/logos/1.png",
            3, "Seed 3", "/logos/3.png",
            new DateTime(2026, 6, 8, 18, 0, 0),
            MatchStage.PlayoffFinal,
            1,
            3);
        var thirdPlace = new Solution.UniLeague.Core.Domain.Match(
            leagueId, 4,
            4, "Seed 4", "/logos/4.png",
            2, "Seed 2", "/logos/2.png",
            new DateTime(2026, 6, 8, 16, 0, 0),
            MatchStage.PlayoffThirdPlace,
            4,
            2);

        var repo = new Mock<IMatchRepository>();
        repo.Setup(r => r.GetScheduleByLeague(leagueId))
            .Returns(new PagedResult<Solution.UniLeague.Core.Domain.Match>(
                new List<Solution.UniLeague.Core.Domain.Match> { semifinal, final, thirdPlace }, 3));

        var result = CreateService(repo).GetScheduleByLeague(leagueId);

        result[0].Stage.ShouldBe("PlayoffSemifinal");
        result[0].IsPlayoff.ShouldBeTrue();
        result[0].PlayoffRoundLabel.ShouldBe("Semifinal");
        result[0].HomeSeed.ShouldBe(1);
        result[0].AwaySeed.ShouldBe(4);
        result[1].Stage.ShouldBe("PlayoffFinal");
        result[1].PlayoffRoundLabel.ShouldBe("Final");
        result[2].Stage.ShouldBe("PlayoffThirdPlace");
        result[2].PlayoffRoundLabel.ShouldBe("Third Place");
    }

    [Fact]
    public void Filters_matches_by_league_id()
    {
        var leagueId = 5L;
        var matches = new List<Solution.UniLeague.Core.Domain.Match>
        {
            CreateMatch(1, leagueId, 1, "Team A", "Team B"),
            CreateMatch(2, leagueId, 1, "Team C", "Team D")
        };

        var repo = new Mock<IMatchRepository>();
        repo.Setup(r => r.GetScheduleByLeague(leagueId))
            .Returns(new PagedResult<Solution.UniLeague.Core.Domain.Match>(matches, matches.Count));

        var result = CreateService(repo).GetScheduleByLeague(leagueId);

        result.ShouldNotBeNull();
        result.All(m => m.LeagueId == leagueId).ShouldBeTrue();
        repo.Verify(r => r.GetScheduleByLeague(leagueId), Times.Once);
    }

    [Fact]
    public void GetResults_returns_only_matches_with_result()
    {
        var leagueId = 1L;
        var matchWithResult = CreateMatch(1, leagueId, 1, "Crvena zvezda", "Partizan");
        matchWithResult.SetResult(MatchResult.Create(2, 1));

        var repo = new Mock<IMatchRepository>();
        repo.Setup(r => r.GetResultsByLeague(leagueId))
            .Returns(new PagedResult<Solution.UniLeague.Core.Domain.Match>(new List<Solution.UniLeague.Core.Domain.Match> { matchWithResult }, 1));

        var result = CreateService(repo).GetResultsByLeague(leagueId);

        result.ShouldNotBeNull();
        result.Count.ShouldBe(1);
        result.All(m => m.Result != null).ShouldBeTrue();
    }

    [Fact]
    public void GetResults_maps_result_to_correct_format()
    {
        var leagueId = 1L;
        var match = CreateMatch(1, leagueId, 1, "Crvena zvezda", "Partizan");
        match.SetResult(MatchResult.Create(3, 0));

        var repo = new Mock<IMatchRepository>();
        repo.Setup(r => r.GetResultsByLeague(leagueId))
            .Returns(new PagedResult<Solution.UniLeague.Core.Domain.Match>(new List<Solution.UniLeague.Core.Domain.Match> { match }, 1));

        var result = CreateService(repo).GetResultsByLeague(leagueId);

        result[0].Result.ShouldBe("3:0");
    }

    [Fact]
    public void GetResults_returns_empty_list_when_no_results()
    {
        var repo = new Mock<IMatchRepository>();
        repo.Setup(r => r.GetResultsByLeague(It.IsAny<long>()))
            .Returns(new PagedResult<Solution.UniLeague.Core.Domain.Match>(new List<Solution.UniLeague.Core.Domain.Match>(), 0));

        var result = CreateService(repo).GetResultsByLeague(999L);

        result.ShouldNotBeNull();
        result.Count.ShouldBe(0);
    }

    [Fact]
    public void GetResults_maps_quarter_scores_when_present()
    {
        var leagueId = 1L;
        var match = CreateMatch(1, leagueId, 1, "Crvena zvezda KK", "Partizan KK");
        var quarters = new[]
        {
            QuarterScore.Create(1, 25, 21),
            QuarterScore.Create(2, 22, 25),
            QuarterScore.Create(3, 30, 28),
            QuarterScore.Create(4, 25, 21),
        };
        match.SetResult(MatchResult.CreateWithQuarters(102, 95, quarters));

        var repo = new Mock<IMatchRepository>();
        repo.Setup(r => r.GetResultsByLeague(leagueId))
            .Returns(new PagedResult<Solution.UniLeague.Core.Domain.Match>(new List<Solution.UniLeague.Core.Domain.Match> { match }, 1));

        var result = CreateService(repo).GetResultsByLeague(leagueId);

        result[0].Result.ShouldBe("102:95");
        result[0].Quarters.ShouldNotBeNull();
        result[0].Quarters!.Count.ShouldBe(4);
        result[0].Quarters![0].QuarterNumber.ShouldBe(1);
        result[0].Quarters![0].HomeScore.ShouldBe(25);
        result[0].Quarters![0].AwayScore.ShouldBe(21);
    }

    [Fact]
    public void GetResults_returns_null_quarters_for_matches_without_quarters()
    {
        var leagueId = 1L;
        var match = CreateMatch(1, leagueId, 1, "Partizan", "Vojvodina");
        match.SetResult(MatchResult.Create(2, 1));

        var repo = new Mock<IMatchRepository>();
        repo.Setup(r => r.GetResultsByLeague(leagueId))
            .Returns(new PagedResult<Solution.UniLeague.Core.Domain.Match>(new List<Solution.UniLeague.Core.Domain.Match> { match }, 1));

        var result = CreateService(repo).GetResultsByLeague(leagueId);

        result[0].Result.ShouldBe("2:1");
        result[0].Quarters.ShouldBeNull();
    }

    [Fact]
    public void GetResults_maps_set_scores_correctly()
    {
        var leagueId = 1L;
        var match = CreateMatch(1, leagueId, 1, "Vojvodina Ribarska", "Spartak Subotica");
        var sets = new[]
        {
            SetScore.Create(1, 25, 21),
            SetScore.Create(2, 22, 25),
            SetScore.Create(3, 25, 18),
            SetScore.Create(4, 25, 19),
        };
        match.SetResult(MatchResult.CreateWithSets(3, 1, sets));

        var repo = new Mock<IMatchRepository>();
        repo.Setup(r => r.GetResultsByLeague(leagueId))
            .Returns(new PagedResult<Solution.UniLeague.Core.Domain.Match>(new List<Solution.UniLeague.Core.Domain.Match> { match }, 1));

        var result = CreateService(repo).GetResultsByLeague(leagueId);

        var dto = result[0];
        dto.Result.ShouldBe("3:1");
        dto.Sets.ShouldNotBeNull();
        dto.Sets!.Count.ShouldBe(4);
        dto.Sets![0].SetNumber.ShouldBe(1);
        dto.Sets![0].HomeScore.ShouldBe(25);
        dto.Sets![0].AwayScore.ShouldBe(21);
        dto.Quarters.ShouldBeNull();
    }

    [Fact]
    public void GetResults_returns_null_sets_for_matches_without_sets()
    {
        var leagueId = 1L;
        var match = CreateMatch(1, leagueId, 1, "Partizan", "Vojvodina");
        match.SetResult(MatchResult.Create(2, 1));

        var repo = new Mock<IMatchRepository>();
        repo.Setup(r => r.GetResultsByLeague(leagueId))
            .Returns(new PagedResult<Solution.UniLeague.Core.Domain.Match>(new List<Solution.UniLeague.Core.Domain.Match> { match }, 1));

        var result = CreateService(repo).GetResultsByLeague(leagueId);

        result[0].Sets.ShouldBeNull();
        result[0].Quarters.ShouldBeNull();
    }

    [Fact]
    public void GetResults_throws_when_sets_do_not_match_result()
    {
        var match = CreateMatch(1, 1L, 1, "Vojvodina Ribarska", "Spartak Subotica");
        var sets = new[]
        {
            SetScore.Create(1, 25, 21),
            SetScore.Create(2, 25, 18),
            SetScore.Create(3, 25, 19),
        };

        Should.Throw<ArgumentException>(() =>
            match.SetResult(MatchResult.CreateWithSets(2, 1, sets)));
    }

    [Fact]
    public void GetResults_sets_result_without_service_call_correctly()
    {
        var match = CreateMatch(1, 1L, 1, "Vojvodina Ribarska", "Spartak Subotica");
        var sets = new[]
        {
            SetScore.Create(1, 25, 21),
            SetScore.Create(2, 22, 25),
            SetScore.Create(3, 25, 18),
            SetScore.Create(4, 25, 19),
        };
        match.SetResult(MatchResult.CreateWithSets(3, 1, sets));

        match.HasResult.ShouldBeTrue();
        match.Result!.HasSets.ShouldBeTrue();
        match.Result!.HasQuarters.ShouldBeFalse();
        match.Result!.Sets.Count.ShouldBe(4);
        match.Result!.ToString().ShouldBe("3:1");
    }

    [Fact]
    public void GetResults_maps_goal_events_when_present()
    {
        var leagueId = 1L;
        var match = CreateMatch(1, leagueId, 2, "Partizan", "Vojvodina");
        var goals = new[]
        {
            GoalEvent.Create("Natcho",  "Partizan",  true,  23),
            GoalEvent.Create("Šljivić", "Vojvodina", false, 45),
            GoalEvent.Create("Mendy",   "Partizan",  true,  67),
        };
        match.SetResult(MatchResult.CreateWithGoals(2, 1, goals));

        var repo = new Mock<IMatchRepository>();
        repo.Setup(r => r.GetResultsByLeague(leagueId))
            .Returns(new PagedResult<Solution.UniLeague.Core.Domain.Match>(new List<Solution.UniLeague.Core.Domain.Match> { match }, 1));

        var result = CreateService(repo).GetResultsByLeague(leagueId);

        result[0].Result.ShouldBe("2:1");
        result[0].Goals.ShouldNotBeNull();
        result[0].Goals!.Count.ShouldBe(3);
        result[0].Goals![0].Minute.ShouldBe(23);
        result[0].Goals![0].ScorerName.ShouldBe("Natcho");
        result[0].Goals![0].IsHomeTeamGoal.ShouldBeTrue();
        result[0].Goals![1].Minute.ShouldBe(45);
        result[0].Goals![1].ScorerName.ShouldBe("Šljivić");
        result[0].Goals![1].IsHomeTeamGoal.ShouldBeFalse();
        result[0].Goals![2].Minute.ShouldBe(67);
        result[0].Goals![2].ScorerName.ShouldBe("Mendy");
        result[0].Goals![2].IsHomeTeamGoal.ShouldBeTrue();
        result[0].Quarters.ShouldBeNull();
        result[0].Sets.ShouldBeNull();
    }

    [Fact]
    public void GetResults_goals_are_sorted_chronologically()
    {
        var leagueId = 1L;
        var match = CreateMatch(1, leagueId, 2, "Partizan", "Vojvodina");
        var goals = new[]
        {
            GoalEvent.Create("Mendy",   "Partizan",  true,  67),
            GoalEvent.Create("Natcho",  "Partizan",  true,  23),
            GoalEvent.Create("Šljivić", "Vojvodina", false, 45),
        };
        match.SetResult(MatchResult.CreateWithGoals(2, 1, goals));

        var repo = new Mock<IMatchRepository>();
        repo.Setup(r => r.GetResultsByLeague(leagueId))
            .Returns(new PagedResult<Solution.UniLeague.Core.Domain.Match>(new List<Solution.UniLeague.Core.Domain.Match> { match }, 1));

        var mappedGoals = CreateService(repo).GetResultsByLeague(leagueId)[0].Goals!;

        mappedGoals[0].Minute.ShouldBe(23);
        mappedGoals[1].Minute.ShouldBe(45);
        mappedGoals[2].Minute.ShouldBe(67);
    }

    [Fact]
    public void GetResults_returns_null_goals_for_non_football_match()
    {
        var leagueId = 1L;
        var match = CreateMatch(1, leagueId, 1, "Crvena zvezda KK", "Partizan KK");
        var quarters = new[]
        {
            QuarterScore.Create(1, 25, 21),
            QuarterScore.Create(2, 22, 25),
            QuarterScore.Create(3, 30, 28),
            QuarterScore.Create(4, 25, 21),
        };
        match.SetResult(MatchResult.CreateWithQuarters(102, 95, quarters));

        var repo = new Mock<IMatchRepository>();
        repo.Setup(r => r.GetResultsByLeague(leagueId))
            .Returns(new PagedResult< Solution.UniLeague.Core.Domain.Match>(new List<Solution.UniLeague.Core.Domain.Match> { match }, 1));

        var result = CreateService(repo).GetResultsByLeague(leagueId);

        result[0].Goals.ShouldBeNull();
        result[0].Quarters.ShouldNotBeNull();
    }

    //Helper

    private static LeagueService CreateService(Mock<IMatchRepository> repo)
    {
        var mapper = new MapperConfiguration(cfg => cfg.AddProfile<UniLeagueProfile>())
            .CreateMapper();
        var topScorerQueryService = new Mock<ITopScorerQueryService>();
        var playoffService = new Mock<IPlayoffService>();
        return new LeagueService(repo.Object, mapper, topScorerQueryService.Object, playoffService.Object); 
    }

    private static Solution.UniLeague.Core.Domain.Match CreateMatch(long id, long leagueId, int roundNumber,
        string homeTeam, string awayTeam)
    {
        return CreateMatchDetailed(id, leagueId, roundNumber,
            id * 10, homeTeam, "/logo.png",
            id * 10 + 1, awayTeam, "/logo.png",
            DateTime.Now);
    }

    private static Solution.UniLeague.Core.Domain.Match CreateMatchDetailed(
        long id, long leagueId, int roundNumber,
        long homeTeamId, string homeTeamName, string homeTeamLogoUrl,
        long awayTeamId, string awayTeamName, string awayTeamLogoUrl,
        DateTime scheduledAt)
    {
        var match = new Solution.UniLeague.Core.Domain.Match(
            leagueId, roundNumber,
            homeTeamId, homeTeamName, homeTeamLogoUrl,
            awayTeamId, awayTeamName, awayTeamLogoUrl,
            scheduledAt);

        typeof(Solution.UniLeague.Core.Domain.Match).BaseType!
            .GetProperty("Id")!
            .SetValue(match, id);

        return match;
    }
}
