using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.DependencyInjection;
using Shouldly;
using Solution.API.Controllers.Public;
using Solution.UniLeague.API.Dtos;
using Solution.UniLeague.API.Public;

namespace Solution.UniLeague.Tests.Integration;

[Collection("Sequential")]
public class ScheduleQueryTests : BaseUniLeagueIntegrationTest
{
    public ScheduleQueryTests(UniLeagueTestFactory factory) : base(factory) { }

    [Fact]
    public void Retrieves_schedule_for_existing_league()
    {
        // Arrange
        using var scope = Factory.Services.CreateScope();
        var controller = CreateController(scope);

        // Act
        var result = ((ObjectResult)controller.GetSchedule(-1).Result)?.Value as List<MatchDto>;

        // Assert
        result.ShouldNotBeNull();
        result.Count.ShouldBe(4);
        result.All(m => m.LeagueId == -1).ShouldBeTrue();
    }

    [Fact]
    public void Returns_matches_sorted_by_round_then_by_date()
    {
        // Arrange
        using var scope = Factory.Services.CreateScope();
        var controller = CreateController(scope);

        // Act
        var result = ((ObjectResult)controller.GetSchedule(-1).Result)?.Value as List<MatchDto>;

        // Assert
        result.ShouldNotBeNull();
        result[0].RoundNumber.ShouldBe(1);
        result[1].RoundNumber.ShouldBe(1);
        result[2].RoundNumber.ShouldBe(2);
        result[3].RoundNumber.ShouldBe(2);
        result[0].ScheduledAt.ShouldBeLessThan(result[1].ScheduledAt);
    }

    [Fact]
    public void Returns_empty_list_for_unknown_league()
    {
        // Arrange
        using var scope = Factory.Services.CreateScope();
        var controller = CreateController(scope);

        // Act
        var result = ((ObjectResult)controller.GetSchedule(-9999).Result)?.Value as List<MatchDto>;

        // Assert
        result.ShouldNotBeNull();
        result.Count.ShouldBe(0);
    }

    [Fact]
    public void Each_match_contains_required_fields()
    {
        // Arrange
        using var scope = Factory.Services.CreateScope();
        var controller = CreateController(scope);

        // Act
        var result = ((ObjectResult)controller.GetSchedule(-1).Result)?.Value as List<MatchDto>;

        // Assert
        result.ShouldNotBeNull();
        foreach (var match in result)
        {
            match.HomeTeamName.ShouldNotBeNullOrWhiteSpace();
            match.AwayTeamName.ShouldNotBeNullOrWhiteSpace();
            match.ScheduledAt.ShouldBeGreaterThan(DateTime.MinValue);
            match.RoundNumber.ShouldBeGreaterThan(0);
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