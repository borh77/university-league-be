using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.DependencyInjection;
using Shouldly;
using Solution.API.Controllers.Public;
using Solution.UniLeague.API.Dtos;
using Solution.UniLeague.API.Public;
using Xunit;

namespace Solution.UniLeague.Tests.Integration;

[Collection("Sequential")]
public class SetResultQueryTests : BaseUniLeagueIntegrationTest
{
    public SetResultQueryTests(UniLeagueTestFactory factory) : base(factory) { }

    [Fact]
    public void Volleyball_results_have_sets()
    {
       
        using var scope = Factory.Services.CreateScope();
        var controller = CreateController(scope);

        var result = ((ObjectResult)controller.GetResults(-3).Result)?.Value as List<MatchDto>;

        result.ShouldNotBeNull();
        result.Count.ShouldBe(2);
        result.All(m => m.Sets != null && m.Sets.Count > 0).ShouldBeTrue();
    }

    [Fact]
    public void Volleyball_results_have_no_quarters()
    {
        using var scope = Factory.Services.CreateScope();
        var controller = CreateController(scope);

        var result = ((ObjectResult)controller.GetResults(-3).Result)?.Value as List<MatchDto>;

        result.ShouldNotBeNull();
        result.All(m => m.Quarters == null || m.Quarters.Count == 0).ShouldBeTrue();
    }

    [Fact]
    public void Football_results_have_no_sets()
    {
      
        using var scope = Factory.Services.CreateScope();
        var controller = CreateController(scope);

        var result = ((ObjectResult)controller.GetResults(-1).Result)?.Value as List<MatchDto>;

        result.ShouldNotBeNull();
        result.All(m => m.Sets == null || m.Sets.Count == 0).ShouldBeTrue();
    }

    [Fact]
    public void Basketball_results_have_no_sets()
    {
        
        using var scope = Factory.Services.CreateScope();
        var controller = CreateController(scope);

        var result = ((ObjectResult)controller.GetResults(-2).Result)?.Value as List<MatchDto>;

        result.ShouldNotBeNull();
        result.All(m => m.Sets == null || m.Sets.Count == 0).ShouldBeTrue();
        result.All(m => m.Quarters != null && m.Quarters.Count == 4).ShouldBeTrue();
    }

    [Fact]
    public void Volleyball_sets_are_in_chronological_order()
    {
        using var scope = Factory.Services.CreateScope();
        var controller = CreateController(scope);

        var result = ((ObjectResult)controller.GetResults(-3).Result)?.Value as List<MatchDto>;

        result.ShouldNotBeNull();
        foreach (var match in result)
        {
            match.Sets.ShouldNotBeNull();
            for (int i = 0; i < match.Sets!.Count; i++)
                match.Sets[i].SetNumber.ShouldBe(i + 1);
        }
    }

    [Fact]
    public void Four_set_match_has_correct_set_scores()
    {
        // Meč -7: Vojvodina 3:1 Spartak Subotica
        // Setovi: 25:21, 22:25, 25:18, 25:19
        using var scope = Factory.Services.CreateScope();
        var controller = CreateController(scope);

        var result = ((ObjectResult)controller.GetResults(-3).Result)?.Value as List<MatchDto>;

        result.ShouldNotBeNull();
        var match = result.First(m => m.Result == "3:1");

        match.Sets!.Count.ShouldBe(4);
        match.Sets[0].HomeScore.ShouldBe(25);
        match.Sets[0].AwayScore.ShouldBe(21);
        match.Sets[1].HomeScore.ShouldBe(22);
        match.Sets[1].AwayScore.ShouldBe(25);
        match.Sets[2].HomeScore.ShouldBe(25);
        match.Sets[2].AwayScore.ShouldBe(18);
        match.Sets[3].HomeScore.ShouldBe(25);
        match.Sets[3].AwayScore.ShouldBe(19);
    }

    [Fact]
    public void Five_set_match_has_correct_set_scores()
    {
        
        using var scope = Factory.Services.CreateScope();
        var controller = CreateController(scope);

        var result = ((ObjectResult)controller.GetResults(-3).Result)?.Value as List<MatchDto>;

        result.ShouldNotBeNull();
        var match = result.First(m => m.Result == "3:2");

        match.Sets!.Count.ShouldBe(5);
        match.Sets[4].SetNumber.ShouldBe(5);
        match.Sets[4].HomeScore.ShouldBe(15);
        match.Sets[4].AwayScore.ShouldBe(12);
    }

    [Fact]
    public void Volleyball_match_contains_all_required_fields()
    {
        using var scope = Factory.Services.CreateScope();
        var controller = CreateController(scope);

        var result = ((ObjectResult)controller.GetResults(-3).Result)?.Value as List<MatchDto>;

        result.ShouldNotBeNull();
        foreach (var match in result)
        {
            match.HomeTeamName.ShouldNotBeNullOrWhiteSpace();
            match.AwayTeamName.ShouldNotBeNullOrWhiteSpace();
            match.HomeTeamLogoUrl.ShouldNotBeNullOrWhiteSpace();
            match.AwayTeamLogoUrl.ShouldNotBeNullOrWhiteSpace();
            match.Result.ShouldNotBeNullOrWhiteSpace();
            match.ScheduledAt.ShouldBeGreaterThan(DateTime.MinValue);
            match.RoundNumber.ShouldBeGreaterThan(0);
            match.Sets.ShouldNotBeNull();
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
            ControllerContext = BuildContext("-3")
        };
    }
}