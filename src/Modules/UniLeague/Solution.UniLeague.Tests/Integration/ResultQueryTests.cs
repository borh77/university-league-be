using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.DependencyInjection;
using Shouldly;
using Solution.API.Controllers.Public;
using Solution.UniLeague.API.Dtos;
using Solution.UniLeague.API.Public;

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

    private static ScheduleController CreateController(IServiceScope scope)
    {
        return new ScheduleController(
            scope.ServiceProvider.GetRequiredService<ILeagueService>())
        {
            ControllerContext = BuildContext("-1")
        };
    }
}