using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Shouldly;
using Solution.API.Controllers.Public;
using Solution.UniLeague.API.Dtos;
using Solution.UniLeague.API.Public;
using Solution.UniLeague.Infrastructure.Database;

namespace Solution.UniLeague.Tests.Integration;

[Collection("Sequential")]
public class ResultsQueryTests : BaseUniLeagueIntegrationTest
{
    public ResultsQueryTests(UniLeagueTestFactory factory) : base(factory) { }

    [Fact]
    public void Retrieves_only_matches_with_result()
    {
        // Arrange
        using var scope = Factory.Services.CreateScope();
        var controller = CreateController(scope);
    

        // Act
        var result = ((ObjectResult)controller.GetResults(-1).Result)?.Value as List<MatchDto>;

        // Assert
        result.ShouldNotBeNull();
        result.Count.ShouldBe(2);
        result.All(m => m.Result != null).ShouldBeTrue();
    }

    [Fact]
    public void Result_format_is_correct()
    {
        // Arrange
        using var scope = Factory.Services.CreateScope();
        var controller = CreateController(scope);

        // Act
        var result = ((ObjectResult)controller.GetResults(-1).Result)?.Value as List<MatchDto>;

        // Assert
        result.ShouldNotBeNull();
        result.Any(m => m.Result == "2:1").ShouldBeTrue();
        result.Any(m => m.Result == "0:3").ShouldBeTrue();
    }

    [Fact]
    public void Returns_matches_sorted_by_round_then_by_date()
    {
        // Arrange
        using var scope = Factory.Services.CreateScope();
        var controller = CreateController(scope);

        // Act
        var result = ((ObjectResult)controller.GetResults(-1).Result)?.Value as List<MatchDto>;

        // Assert
        result.ShouldNotBeNull();
        result[0].ScheduledAt.ShouldBeLessThanOrEqualTo(result[1].ScheduledAt);
    }

    [Fact]
    public void Each_match_contains_required_fields()
    {
        // Arrange
        using var scope = Factory.Services.CreateScope();
        var controller = CreateController(scope);

        // Act
        var result = ((ObjectResult)controller.GetResults(-1).Result)?.Value as List<MatchDto>;

        // Assert
        result.ShouldNotBeNull();
        foreach (var match in result)
        {
            match.HomeTeamName.ShouldNotBeNullOrWhiteSpace();
            match.AwayTeamName.ShouldNotBeNullOrWhiteSpace();
            match.ScheduledAt.ShouldBeGreaterThan(DateTime.MinValue);
            match.RoundNumber.ShouldBeGreaterThan(0);
            match.Result.ShouldNotBeNullOrWhiteSpace();
        }
    }

    [Fact]
    public void Returns_empty_list_for_unknown_league()
    {
        // Arrange
        using var scope = Factory.Services.CreateScope();
        var controller = CreateController(scope);

        // Act
        var result = ((ObjectResult)controller.GetResults(-99999).Result)?.Value as List<MatchDto>;

        // Assert
        result.ShouldNotBeNull();
        result.Count.ShouldBe(0);
    }

    [Fact]
    public void Football_results_have_no_quarters()
    {
        //Arrange 
        using var scope = Factory.Services.CreateScope();
        var controller = CreateController(scope);

        // Act
        var result = ((ObjectResult)controller.GetResults(-1).Result)?.Value as List<MatchDto>;

        // Assert
        result.ShouldNotBeNull();
        result.All(m => m.Quarters == null || m.Quarters.Count == 0).ShouldBeTrue();
    }

    [Fact]
    public void Basketball_results_have_quarters()
    {
        //Arrange
        using var scope = Factory.Services.CreateScope();
        var controller = CreateController(scope);

        // Act
        var result = ((ObjectResult)controller.GetResults(-2).Result)?.Value as List<MatchDto>;

        //Assert
        result.ShouldNotBeNull();
        result.Count.ShouldBe(2);
        result.All(m => m.Quarters != null && m.Quarters.Count == 4).ShouldBeTrue();
    }

    [Fact]
    public void Basketball_quarters_are_in_chronological_order()
    {
        //Arrange
        using var scope = Factory.Services.CreateScope();
        var controller = CreateController(scope);
        
        //Act
        var result = ((ObjectResult)controller.GetResults(-2).Result)?.Value as List<MatchDto>;

        //Assert
        result.ShouldNotBeNull();
        foreach (var match in result)
        {
            match.Quarters.ShouldNotBeNull();
            for (int i = 0; i < match.Quarters!.Count; i++)
                match.Quarters[i].QuarterNumber.ShouldBe(i + 1);
        }
    }

    [Fact]
    public void Basketball_quarter_scores_match_test_data()
    {
        //Arrange
        using var scope = Factory.Services.CreateScope();
        var controller = CreateController(scope);
        
        //Act
        var result = ((ObjectResult)controller.GetResults(-2).Result)?.Value as List<MatchDto>;
       
        //Assert
        result.ShouldNotBeNull();
        var match = result.First(m => m.Result == "102:95");

        match.Quarters![0].HomeScore.ShouldBe(25);
        match.Quarters![0].AwayScore.ShouldBe(21);
        match.Quarters![1].HomeScore.ShouldBe(22);
        match.Quarters![1].AwayScore.ShouldBe(25);
        match.Quarters![2].HomeScore.ShouldBe(30);
        match.Quarters![2].AwayScore.ShouldBe(28);
        match.Quarters![3].HomeScore.ShouldBe(25);
        match.Quarters![3].AwayScore.ShouldBe(21);
    }

    [Fact]
    public void Basketball_match_contains_all_required_fields()
    {
        //Arrange
        using var scope = Factory.Services.CreateScope();
        var controller = CreateController(scope);

        //Act
        var result = ((ObjectResult)controller.GetResults(-2).Result)?.Value as List<MatchDto>;

        //Assert
        result.ShouldNotBeNull();
        foreach (var match in result)
        {
            match.HomeTeamName.ShouldNotBeNullOrWhiteSpace();
            match.AwayTeamName.ShouldNotBeNullOrWhiteSpace();
            match.Result.ShouldNotBeNullOrWhiteSpace();
            match.ScheduledAt.ShouldBeGreaterThan(DateTime.MinValue);
            match.RoundNumber.ShouldBeGreaterThan(0);
            match.Quarters.ShouldNotBeNull();
        }
    }

    private static ScheduleController CreateController(IServiceScope scope)
    {
        return new ScheduleController(
            scope.ServiceProvider.GetRequiredService<ILeagueService>())
        {
            ControllerContext = BuildContext("-1")
        };
    }
}