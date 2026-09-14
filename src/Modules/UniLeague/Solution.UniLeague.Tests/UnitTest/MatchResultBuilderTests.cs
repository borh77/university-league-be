using Moq;
using Shouldly;
using Solution.UniLeague.API.Dtos;
using Solution.UniLeague.Core.Domain;
using Solution.UniLeague.Core.Domain.RepositoryInterfaces;
using Solution.UniLeague.Core.UseCases;
using Match = Solution.UniLeague.Core.Domain.Match;

namespace Solution.UniLeague.Tests.UnitTest;

public class MatchResultBuilderTests
{
    [Fact]
    public void Build_basketball_without_player_stats_uses_quarters_only()
    {
        var builder = new MatchResultBuilder(new Mock<ITeamRepository>().Object);
        var match = CreateMatch();

        var result = builder.Build(Sport.Basketball, match, new SubmitMatchResultDto
        {
            HomeScore = 55,
            AwayScore = 48,
            Quarters = new List<QuarterScoreDto>
            {
                new() { QuarterNumber = 1, HomeScore = 30, AwayScore = 20 },
                new() { QuarterNumber = 2, HomeScore = 25, AwayScore = 28 },
            },
        });

        result.HasPlayerStats.ShouldBeFalse();
        result.Quarters.Count.ShouldBe(2);
    }

    [Fact]
    public void Build_volleyball_without_player_stats_uses_sets_only()
    {
        var builder = new MatchResultBuilder(new Mock<ITeamRepository>().Object);
        var match = CreateMatch();

        var result = builder.Build(Sport.Volleyball, match, new SubmitMatchResultDto
        {
            HomeScore = 2,
            AwayScore = 0,
            Sets = new List<SetScoreDto>
            {
                new() { SetNumber = 1, HomeScore = 25, AwayScore = 20 },
                new() { SetNumber = 2, HomeScore = 25, AwayScore = 18 },
            },
        });

        result.HasPlayerStats.ShouldBeFalse();
        result.Sets.Count.ShouldBe(2);
    }

    [Fact]
    public void Build_basketball_without_quarters_still_throws()
    {
        var builder = new MatchResultBuilder(new Mock<ITeamRepository>().Object);
        var match = CreateMatch();

        Should.Throw<ArgumentException>(() =>
            builder.Build(Sport.Basketball, match, new SubmitMatchResultDto { HomeScore = 1, AwayScore = 0 }));
    }

    [Fact]
    public void Build_basketball_with_mismatched_player_stats_still_throws()
    {
        var homeTeam = new Team("Home", null)
        {
            Players = new List<Player> { new(10L, "Marko", "Markovic", 7, null) },
        };
        var awayTeam = new Team("Away", null) { Players = new List<Player>() };
        SetId(homeTeam.Players[0], 1);

        var teamRepo = new Mock<ITeamRepository>();
        teamRepo.Setup(r => r.GetByIdWithPlayers(10L)).Returns(homeTeam);
        teamRepo.Setup(r => r.GetByIdWithPlayers(20L)).Returns(awayTeam);

        var builder = new MatchResultBuilder(teamRepo.Object);
        var match = CreateMatch();

        Should.Throw<ArgumentException>(() => builder.Build(Sport.Basketball, match, new SubmitMatchResultDto
        {
            HomeScore = 10,
            AwayScore = 0,
            Quarters = new List<QuarterScoreDto>
            {
                new() { QuarterNumber = 1, HomeScore = 10, AwayScore = 0 },
            },
            PlayerStats = new List<PlayerStatInputDto>
            {
                new() { PlayerId = 1, IsHomeTeam = true, Points = 4 },
            },
        }));
    }

    private static Match CreateMatch()
        => new(500L, 1, 10L, "Home", "", 20L, "Away", "", DateTime.UtcNow);

    private static void SetId(Player player, long id)
        => typeof(Player).GetProperty(nameof(Player.Id))!.SetValue(player, id);
}
