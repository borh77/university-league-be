using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.DependencyInjection;
using Shouldly;
using Solution.API.Controllers.Public;
using Solution.UniLeague.API.Dtos;
using Solution.UniLeague.API.Public;
using Xunit;

namespace Solution.UniLeague.Tests.Integration;

[Collection("Sequential")]
public class GoalResultQueryTests : BaseUniLeagueIntegrationTest
{
    public GoalResultQueryTests(UniLeagueTestFactory factory) : base(factory) { }

    [Fact]
    public void Football_results_have_goals()
    {
        using var scope = Factory.Services.CreateScope();
        var controller = CreateController(scope);

        var result = ((ObjectResult)controller.GetResults(-1).Result)?.Value as List<MatchDto>;

        result.ShouldNotBeNull();
        result.Count.ShouldBe(2);
        result.All(m => m.Goals != null && m.Goals.Count > 0).ShouldBeTrue();
    }

    [Fact]
    public void Football_results_have_no_quarters_or_sets()
    {
        using var scope = Factory.Services.CreateScope();
        var controller = CreateController(scope);

        var result = ((ObjectResult)controller.GetResults(-1).Result)?.Value as List<MatchDto>;

        result.ShouldNotBeNull();
        result.All(m => m.Quarters == null || m.Quarters.Count == 0).ShouldBeTrue();
        result.All(m => m.Sets == null || m.Sets.Count == 0).ShouldBeTrue();
    }

    [Fact]
    public void Basketball_results_have_no_goals()
    {
        using var scope = Factory.Services.CreateScope();
        var controller = CreateController(scope);

        var result = ((ObjectResult)controller.GetResults(-2).Result)?.Value as List<MatchDto>;

        result.ShouldNotBeNull();
        result.All(m => m.Goals == null || m.Goals.Count == 0).ShouldBeTrue();
        result.All(m => m.Quarters != null && m.Quarters.Count == 4).ShouldBeTrue();
    }

    [Fact]
    public void Volleyball_results_have_no_goals()
    {
        using var scope = Factory.Services.CreateScope();
        var controller = CreateController(scope);

        var result = ((ObjectResult)controller.GetResults(-3).Result)?.Value as List<MatchDto>;

        result.ShouldNotBeNull();
        result.All(m => m.Goals == null || m.Goals.Count == 0).ShouldBeTrue();
        result.All(m => m.Sets != null && m.Sets.Count > 0).ShouldBeTrue();
    }

    [Fact]
    public void Goals_are_sorted_chronologically_by_minute()
    {
        using var scope = Factory.Services.CreateScope();
        var controller = CreateController(scope);

        var result = ((ObjectResult)controller.GetResults(-1).Result)?.Value as List<MatchDto>;

        result.ShouldNotBeNull();
        foreach (var match in result)
        {
            match.Goals.ShouldNotBeNull();
            for (int i = 1; i < match.Goals!.Count; i++)
                match.Goals[i].Minute.ShouldBeGreaterThanOrEqualTo(match.Goals[i - 1].Minute);
        }
    }

    [Fact]
    public void Match_2_1_has_correct_goal_data()
    {
        using var scope = Factory.Services.CreateScope();
        var controller = CreateController(scope);

        var result = ((ObjectResult)controller.GetResults(-1).Result)?.Value as List<MatchDto>;

        result.ShouldNotBeNull();
        var match = result.First(m => m.Result == "2:1");

        match.Goals!.Count.ShouldBe(3);

        match.Goals[0].Minute.ShouldBe(23);
        match.Goals[0].ScorerName.ShouldBe("Natcho");
        match.Goals[0].IsHomeTeamGoal.ShouldBeTrue();

        match.Goals[1].Minute.ShouldBe(45);
        match.Goals[1].ScorerName.ShouldBe("Šljivić");
        match.Goals[1].IsHomeTeamGoal.ShouldBeFalse();

        match.Goals[2].Minute.ShouldBe(67);
        match.Goals[2].ScorerName.ShouldBe("Mendy");
        match.Goals[2].IsHomeTeamGoal.ShouldBeTrue();
    }

    [Fact]
    public void Match_0_3_has_only_away_goals()
    {
        using var scope = Factory.Services.CreateScope();
        var controller = CreateController(scope);

        var result = ((ObjectResult)controller.GetResults(-1).Result)?.Value as List<MatchDto>;

        result.ShouldNotBeNull();
        var match = result.First(m => m.Result == "0:3");

        match.Goals!.Count.ShouldBe(3);
        match.Goals.All(g => !g.IsHomeTeamGoal).ShouldBeTrue();
        match.Goals.All(g => g.TeamName == "Crvena zvezda").ShouldBeTrue();
    }

    [Fact]
    public void Each_football_match_contains_all_required_fields()
    {
        using var scope = Factory.Services.CreateScope();
        var controller = CreateController(scope);

        var result = ((ObjectResult)controller.GetResults(-1).Result)?.Value as List<MatchDto>;

        result.ShouldNotBeNull();
        foreach (var match in result)
        {
            match.HomeTeamName.ShouldNotBeNullOrWhiteSpace();
            match.AwayTeamName.ShouldNotBeNullOrWhiteSpace();
            match.Result.ShouldNotBeNullOrWhiteSpace();
            match.ScheduledAt.ShouldBeGreaterThan(DateTime.MinValue);
            match.RoundNumber.ShouldBeGreaterThan(0);
            match.Goals.ShouldNotBeNull();

            foreach (var goal in match.Goals!)
            {
                goal.ScorerName.ShouldNotBeNullOrWhiteSpace();
                goal.TeamName.ShouldNotBeNullOrWhiteSpace();
                goal.Minute.ShouldBeGreaterThan(0);
            }
        }
    }

    [Fact]
    public void Returns_empty_list_for_unknown_league()
    {
        using var scope = Factory.Services.CreateScope();
        var controller = CreateController(scope);

        var result = ((ObjectResult)controller.GetResults(-99999).Result)?.Value as List<MatchDto>;

        result.ShouldNotBeNull();
        result.Count.ShouldBe(0);
    }

    //Helper

    private static ScheduleController CreateController(IServiceScope scope)
    {
        return new ScheduleController(
            scope.ServiceProvider.GetRequiredService<ILeagueService>())
        {
            ControllerContext = BuildContext("-1")
        };
    }
}