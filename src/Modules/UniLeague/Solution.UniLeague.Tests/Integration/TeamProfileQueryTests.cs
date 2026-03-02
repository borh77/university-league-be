using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.DependencyInjection;
using Shouldly;
using Solution.API.Controllers.Public;
using Solution.UniLeague.API.Dtos;
using Solution.UniLeague.API.Public;

namespace Solution.UniLeague.Tests.Integration;

[Collection("Sequential")]
public class TeamProfileQueryTests : BaseUniLeagueIntegrationTest
{
    public TeamProfileQueryTests(UniLeagueTestFactory factory) : base(factory) { }

    [Fact]
    public void Retrieves_team_with_players()
    {
        // Arrange
        using var scope = Factory.Services.CreateScope();
        var controller = CreateController(scope);

        // Act
        var result = ((ObjectResult)controller.GetTeamProfile(-10).Result)?.Value as TeamProfileDto;

        // Assert
        result.ShouldNotBeNull();
        result.Id.ShouldBe(-10);
        result.Name.ShouldBe("Crvena zvezda");
        result.Players.Count.ShouldBe(3);
    }

    [Fact]
    public void Each_player_contains_required_fields()
    {
        // Arrange
        using var scope = Factory.Services.CreateScope();
        var controller = CreateController(scope);

        // Act
        var result = ((ObjectResult)controller.GetTeamProfile(-10).Result)?.Value as TeamProfileDto;

        // Assert
        result.ShouldNotBeNull();
        foreach (var player in result.Players)
        {
            player.FirstName.ShouldNotBeNullOrWhiteSpace();
            player.LastName.ShouldNotBeNullOrWhiteSpace();
            player.JerseyNumber.ShouldBeInRange(1, 99);
        }
    }

    [Fact]
    public void Returns_404_for_unknown_team()
    {
        // Arrange
        using var scope = Factory.Services.CreateScope();
        var controller = CreateController(scope);

        // Act
        var result = controller.GetTeamProfile(-99999).Result;

        // Assert
        result.ShouldBeOfType<NotFoundResult>();
    }

    [Fact]
    public void Team_without_players_returns_empty_list()
    {
        // Arrange
        using var scope = Factory.Services.CreateScope();
        var controller = CreateController(scope);

        // Act — tim -13 (Čukarički) nema igrača u seed podacima
        var result = ((ObjectResult)controller.GetTeamProfile(-13).Result)?.Value as TeamProfileDto;

        // Assert
        result.ShouldNotBeNull();
        result.Players.ShouldBeEmpty();
    }

    private static TeamController CreateController(IServiceScope scope)
    {
        return new TeamController(
            scope.ServiceProvider.GetRequiredService<ITeamService>())
        {
            ControllerContext = BuildContext("-1")
        };
    }
}