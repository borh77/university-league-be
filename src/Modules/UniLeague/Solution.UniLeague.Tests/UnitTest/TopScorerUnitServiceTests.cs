using AutoMapper;
using Moq;
using Shouldly;
using Solution.UniLeague.API.Dtos;
using Solution.UniLeague.Core.Domain.RepositoryInterfaces;
using Solution.UniLeague.Core.Mappers;
using Solution.UniLeague.Core.UseCases;
using Xunit;

namespace Solution.UniLeague.Tests.UnitTest;

public class TopScorerServiceTests
{
    [Fact]
    public void Returns_scorers_sorted_by_goals_descending()
    {
        var queryService = new Mock<ITopScorerQueryService>();
        queryService.Setup(s => s.GetTopScorersByLeague(1L)).Returns(new List<TopScorerDto>
        {
            new() { ScorerName = "Natcho",  TeamName = "Partizan",      Goals = 5,},
            new() { ScorerName = "Katai",   TeamName = "Crvena zvezda", Goals = 3,},
            new() { ScorerName = "Šljivić", TeamName = "Vojvodina",     Goals = 1},
        });

        var result = CreateService(queryService.Object).GetTopScorersByLeague(1L);

        result[0].Goals.ShouldBe(5);
        result[1].Goals.ShouldBe(3);
        result[2].Goals.ShouldBe(1);
    }

    [Fact]
    public void Assigns_positions_correctly()
    {
        var queryService = new Mock<ITopScorerQueryService>();
        queryService.Setup(s => s.GetTopScorersByLeague(1L)).Returns(new List<TopScorerDto>
        {
            new() { ScorerName = "Natcho", TeamName = "Partizan",      Goals = 5 },
            new() { ScorerName = "Katai",  TeamName = "Crvena zvezda", Goals = 3 },
            new() { ScorerName = "Mendy",  TeamName = "Partizan",      Goals = 1 },
        });

        var result = CreateService(queryService.Object).GetTopScorersByLeague(1L);

        result[0].Position.ShouldBe(1);
        result[1].Position.ShouldBe(2);
        result[2].Position.ShouldBe(3);
    }

    [Fact]
    public void Players_with_same_goals_get_same_position_and_next_skips()
    {
        var queryService = new Mock<ITopScorerQueryService>();
        queryService.Setup(s => s.GetTopScorersByLeague(1L)).Returns(new List<TopScorerDto>
        {
            new() { ScorerName = "Katai",  TeamName = "Crvena zvezda", Goals = 5 },
            new() { ScorerName = "Natcho", TeamName = "Partizan",      Goals = 5 },
            new() { ScorerName = "Mendy",  TeamName = "Partizan",      Goals = 3 },
        });

        var result = CreateService(queryService.Object).GetTopScorersByLeague(1L);

        result[0].Position.ShouldBe(1);
        result[1].Position.ShouldBe(1);
        result[2].Position.ShouldBe(3); // preskače 2
    }

    [Fact]
    public void Returns_empty_list_when_no_goals()
    {
        var queryService = new Mock<ITopScorerQueryService>();
        queryService.Setup(s => s.GetTopScorersByLeague(It.IsAny<long>()))
            .Returns(new List<TopScorerDto>());

        var result = CreateService(queryService.Object).GetTopScorersByLeague(99L);

        result.ShouldNotBeNull();
        result.Count.ShouldBe(0);
    }

    [Fact]
    public void Returns_empty_list_for_non_football_league()
    {
        var queryService = new Mock<ITopScorerQueryService>();
        queryService.Setup(s => s.GetTopScorersByLeague(2L))
            .Returns(new List<TopScorerDto>());

        var result = CreateService(queryService.Object).GetTopScorersByLeague(2L);

        result.Count.ShouldBe(0);
    }

    [Fact]
    public void Does_not_mutate_order_from_query_service()
    {
        var queryService = new Mock<ITopScorerQueryService>();
        queryService.Setup(s => s.GetTopScorersByLeague(1L)).Returns(new List<TopScorerDto>
        {
            new() { ScorerName = "Aleksić", TeamName = "Vojvodina", Goals = 3 },
            new() { ScorerName = "Žikić",   TeamName = "Partizan",  Goals = 3 },
        });

        var result = CreateService(queryService.Object).GetTopScorersByLeague(1L);

        result[0].ScorerName.ShouldBe("Aleksić");
        result[1].ScorerName.ShouldBe("Žikić");
        result[0].Position.ShouldBe(1);
        result[1].Position.ShouldBe(1);
    }

    private static LeagueService CreateService(ITopScorerQueryService topScorerQueryService)
    {
        var repoMock = new Mock<IMatchRepository>();
        var mapper = new MapperConfiguration(cfg => cfg.AddProfile<UniLeagueProfile>())
            .CreateMapper();
        return new LeagueService(repoMock.Object, mapper, topScorerQueryService); 
    }
}