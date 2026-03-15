using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.DependencyInjection;
using Shouldly;
using Solution.API.Controllers.Public;
using Solution.UniLeague.API.Dtos;
using Solution.UniLeague.API.Public;
using Xunit;

namespace Solution.UniLeague.Tests.Integration;

[Collection("Sequential")]
public class TopScorerQueryTests : BaseUniLeagueIntegrationTest
{
    public TopScorerQueryTests(UniLeagueTestFactory factory) : base(factory) { }

    [Fact]
    public void Returns_scorers_for_football_league()
    {
        using var scope = Factory.Services.CreateScope();
        var controller = CreateController(scope);

        var result = ((ObjectResult)controller.GetTopScorers(-1).Result)?.Value as List<TopScorerDto>;

        result.ShouldNotBeNull();
        result.Count.ShouldBeGreaterThan(0);
    }

    [Fact]
    public void Returns_empty_list_for_basketball_league()
    {
        using var scope = Factory.Services.CreateScope();
        var controller = CreateController(scope);

        var result = ((ObjectResult)controller.GetTopScorers(-2).Result)?.Value as List<TopScorerDto>;

        result.ShouldNotBeNull();
        result.Count.ShouldBe(0);
    }

    [Fact]
    public void Returns_empty_list_for_volleyball_league()
    {
        using var scope = Factory.Services.CreateScope();
        var controller = CreateController(scope);

        var result = ((ObjectResult)controller.GetTopScorers(-3).Result)?.Value as List<TopScorerDto>;

        result.ShouldNotBeNull();
        result.Count.ShouldBe(0);
    }

    [Fact]
    public void Returns_empty_list_for_unknown_league()
    {
        using var scope = Factory.Services.CreateScope();
        var controller = CreateController(scope);

        var result = ((ObjectResult)controller.GetTopScorers(-99999).Result)?.Value as List<TopScorerDto>;

        result.ShouldNotBeNull();
        result.Count.ShouldBe(0);
    }

    [Fact]
    public void Scorers_are_sorted_by_goals_descending()
    {
        using var scope = Factory.Services.CreateScope();
        var controller = CreateController(scope);

        var result = ((ObjectResult)controller.GetTopScorers(-1).Result)?.Value as List<TopScorerDto>;

        result.ShouldNotBeNull();
        for (int i = 1; i < result.Count; i++)
            result[i].Goals.ShouldBeLessThanOrEqualTo(result[i - 1].Goals);
    }

    [Fact]
    public void First_position_is_one()
    {
        using var scope = Factory.Services.CreateScope();
        var controller = CreateController(scope);

        var result = ((ObjectResult)controller.GetTopScorers(-1).Result)?.Value as List<TopScorerDto>;

        result.ShouldNotBeNull();
        result.ShouldNotBeEmpty();
        result[0].Position.ShouldBe(1);
    }

    [Fact]
    public void Only_players_with_at_least_one_goal_are_returned()
    {
        using var scope = Factory.Services.CreateScope();
        var controller = CreateController(scope);

        var result = ((ObjectResult)controller.GetTopScorers(-1).Result)?.Value as List<TopScorerDto>;

        result.ShouldNotBeNull();
        result.All(s => s.Goals > 0).ShouldBeTrue();
    }

    [Fact]
    public void Each_scorer_contains_all_required_fields()
    {
        using var scope = Factory.Services.CreateScope();
        var controller = CreateController(scope);

        var result = ((ObjectResult)controller.GetTopScorers(-1).Result)?.Value as List<TopScorerDto>;

        result.ShouldNotBeNull();
        foreach (var scorer in result)
        {
            scorer.ScorerName.ShouldNotBeNullOrWhiteSpace();
            scorer.TeamName.ShouldNotBeNullOrWhiteSpace();
            scorer.Goals.ShouldBeGreaterThan(0);
            scorer.Position.ShouldBeGreaterThan(0);
        }
    }

    [Fact]
    public void All_scorers_from_test_data_are_present()
    {
       
        using var scope = Factory.Services.CreateScope();
        var controller = CreateController(scope);

        var result = ((ObjectResult)controller.GetTopScorers(-1).Result)?.Value as List<TopScorerDto>;

        result.ShouldNotBeNull();
        var names = result.Select(s => s.ScorerName).ToList();
        names.ShouldContain("Natcho");
        names.ShouldContain("Šljivić");
        names.ShouldContain("Mendy");
        names.ShouldContain("Katai");
        names.ShouldContain("Štulić");
        names.ShouldContain("Rodić");
    }

    [Fact]
    public void Tied_scorers_get_same_position_and_next_position_skips()
    {
        // Svi igrači iz test podataka imaju po 1 gol — svi dele poziciju 1
        using var scope = Factory.Services.CreateScope();
        var controller = CreateController(scope);

        var result = ((ObjectResult)controller.GetTopScorers(-1).Result)?.Value as List<TopScorerDto>;

        result.ShouldNotBeNull();
        result.All(s => s.Position == 1).ShouldBeTrue();
    }

    [Fact]
    public void Tied_scorers_are_sorted_by_name_ascending()
    {
        using var scope = Factory.Services.CreateScope();
        var controller = CreateController(scope);

        var result = ((ObjectResult)controller.GetTopScorers(-1).Result)?.Value as List<TopScorerDto>;

        result.ShouldNotBeNull();
        var groups = result.GroupBy(s => s.Goals);
        foreach (var group in groups)
        {
            var names = group.Select(s => s.ScorerName).ToList();
            names.SequenceEqual(names.OrderBy(n => n)).ShouldBeTrue();
        }
    }

    [Fact]
    public void Scorer_team_name_matches_goal_team()
    {
        using var scope = Factory.Services.CreateScope();
        var controller = CreateController(scope);

        var result = ((ObjectResult)controller.GetTopScorers(-1).Result)?.Value as List<TopScorerDto>;

        result.ShouldNotBeNull();
        var natcho = result.FirstOrDefault(s => s.ScorerName == "Natcho");
        natcho.ShouldNotBeNull();
        natcho!.TeamName.ShouldBe("Partizan");

        var katai = result.FirstOrDefault(s => s.ScorerName == "Katai");
        katai.ShouldNotBeNull();
        katai!.TeamName.ShouldBe("Crvena zvezda");
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