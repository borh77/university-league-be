using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.DependencyInjection;
using Shouldly;
using Solution.UniLeague.API.Dtos;
using Solution.UniLeague.API.Public;
using Solution.UniLeague.Core.Domain;
using Solution.API.Controllers.Public;

namespace Solution.UniLeague.Tests.Integration;

[Collection("Sequential")]
public class StandingsQueryTests : BaseUniLeagueIntegrationTest
{
    public StandingsQueryTests(UniLeagueTestFactory factory) : base(factory) { }

    // ──────────────────────────────────────────────
    // Football
    // ──────────────────────────────────────────────

    [Fact]
    public void Retrieves_football_standings_with_correct_count()
    {
        using var scope = Factory.Services.CreateScope();
        var controller = CreateController(scope);

        var result = ((ObjectResult)controller.GetStandings("football", null).Result)?.Value
            as List<StandingsRowDto>;

        result.ShouldNotBeNull();
        result.Count.ShouldBe(4);
    }

    [Fact]
    public void Football_standings_are_sorted_correctly()
    {
        using var scope = Factory.Services.CreateScope();
        var controller = CreateController(scope);

        var result = ((ObjectResult)controller.GetStandings("football", null).Result)?.Value
            as List<StandingsRowDto>;

        result.ShouldNotBeNull();

        // Points DESC → Difference DESC → Scored DESC
        for (var i = 0; i < result.Count - 1; i++)
        {
            var a = result[i];
            var b = result[i + 1];

            if (a.Points == b.Points)
            {
                if (a.Difference == b.Difference)
                    a.Scored.ShouldBeGreaterThanOrEqualTo(b.Scored);
                else
                    a.Difference.ShouldBeGreaterThan(b.Difference);
            }
            else
            {
                a.Points.ShouldBeGreaterThan(b.Points);
            }
        }
    }

    [Fact]
    public void Football_standings_positions_are_sequential()
    {
        using var scope = Factory.Services.CreateScope();
        var controller = CreateController(scope);

        var result = ((ObjectResult)controller.GetStandings("football", null).Result)?.Value
            as List<StandingsRowDto>;

        result.ShouldNotBeNull();
        for (var i = 0; i < result.Count; i++)
            result[i].Position.ShouldBe(i + 1);
    }

    [Fact]
    public void Football_standings_contain_team_id_and_name()
    {
        using var scope = Factory.Services.CreateScope();
        var controller = CreateController(scope);

        var result = ((ObjectResult)controller.GetStandings("football", null).Result)?.Value
            as List<StandingsRowDto>;

        result.ShouldNotBeNull();
        result.All(r => r.TeamId != 0).ShouldBeTrue();
        result.All(r => !string.IsNullOrWhiteSpace(r.TeamName)).ShouldBeTrue();
    }

    [Fact]
    public void Football_standings_gender_param_is_ignored()
    {
        // gender treba da se ignoriše za non-volleyball
        using var scope = Factory.Services.CreateScope();
        var controller = CreateController(scope);

        var withGender = ((ObjectResult)controller.GetStandings("football", "male").Result)?.Value
            as List<StandingsRowDto>;
        var withoutGender = ((ObjectResult)controller.GetStandings("football", null).Result)?.Value
            as List<StandingsRowDto>;

        withGender.ShouldNotBeNull();
        withoutGender.ShouldNotBeNull();
        withGender.Count.ShouldBe(withoutGender.Count);
    }

    // ──────────────────────────────────────────────
    // Volleyball
    // ──────────────────────────────────────────────

    [Fact]
    public void Retrieves_volleyball_male_standings()
    {
        using var scope = Factory.Services.CreateScope();
        var controller = CreateController(scope);

        var result = ((ObjectResult)controller.GetStandings("volleyball", "male").Result)?.Value
            as List<StandingsRowDto>;

        result.ShouldNotBeNull();
        result.Count.ShouldBeGreaterThan(0);
        result.All(r => r.SetWon.HasValue && r.SetLost.HasValue).ShouldBeTrue();
    }

    [Fact]
    public void Retrieves_volleyball_female_standings()
    {
        using var scope = Factory.Services.CreateScope();
        var controller = CreateController(scope);

        var result = ((ObjectResult)controller.GetStandings("volleyball", "female").Result)?.Value
            as List<StandingsRowDto>;

        result.ShouldNotBeNull();
        result.Count.ShouldBeGreaterThan(0);
    }

    [Fact]
    public void Volleyball_male_and_female_standings_are_independent()
    {
        using var scope = Factory.Services.CreateScope();
        var controller = CreateController(scope);

        var male = ((ObjectResult)controller.GetStandings("volleyball", "male").Result)?.Value
            as List<StandingsRowDto>;
        var female = ((ObjectResult)controller.GetStandings("volleyball", "female").Result)?.Value
            as List<StandingsRowDto>;

        male.ShouldNotBeNull();
        female.ShouldNotBeNull();

        // Timovi iz muške i ženske lige ne smeju biti isti
        var maleIds = male.Select(r => r.TeamId).ToHashSet();
        var femaleIds = female.Select(r => r.TeamId).ToHashSet();
        maleIds.Overlaps(femaleIds).ShouldBeFalse();
    }

    // ──────────────────────────────────────────────
    // Error cases - ISPRAVLJENO
    // ──────────────────────────────────────────────

    [Fact]
    public void Returns_400_for_invalid_sport_string()
    {
        using var scope = Factory.Services.CreateScope();
        var controller = CreateController(scope);

        
        var ex = Should.Throw<ArgumentException>(() =>
            controller.GetStandings("chess", null));

        ex.Message.ShouldContain("Unknown sport: 'chess'");
    }

    [Fact]
    public void Returns_400_for_volleyball_without_gender()
    {
        using var scope = Factory.Services.CreateScope();
        var controller = CreateController(scope);

        Should.Throw<ArgumentException>(() =>
            controller.GetStandings("volleyball", null));
    }

    [Fact]
    public void Returns_400_for_volleyball_with_invalid_gender_value()
    {
        using var scope = Factory.Services.CreateScope();
        var controller = CreateController(scope);

        Should.Throw<ArgumentException>(() =>
            controller.GetStandings("volleyball", "mixed"));
    }

    // ──────────────────────────────────────────────
    // Helper
    // ──────────────────────────────────────────────

    private static StandingsController CreateController(IServiceScope scope)
    {
        return new StandingsController(
            scope.ServiceProvider.GetRequiredService<IStandingsService>())
        {
            ControllerContext = BuildContext("-1")
        };
    }
}