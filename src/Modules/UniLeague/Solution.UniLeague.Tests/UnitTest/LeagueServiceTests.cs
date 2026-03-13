using AutoMapper;
using Moq;
using Shouldly;
using Solution.BuildingBlocks.Core.UseCases;
using Solution.UniLeague.API.Dtos;
using Solution.UniLeague.Core.Domain;
using Solution.UniLeague.Core.Domain.RepositoryInterfaces;
using Solution.UniLeague.Core.Mappers;
using Solution.UniLeague.Core.UseCases;
using Xunit;

namespace Solution.UniLeague.Tests.UnitTest;

public class LeagueServiceTests
{
    [Fact]
    public void Returns_empty_list_when_no_matches_found()
    {
        // Arrange
        var repo = new Mock<IMatchRepository>();
        repo.Setup(r => r.GetScheduleByLeague(It.IsAny<long>()))
            .Returns(new PagedResult<Solution.UniLeague.Core.Domain.Match>(new List<Solution.UniLeague.Core.Domain.Match>(), 0));

        var mapperConfig = new MapperConfiguration(cfg =>
        {
            cfg.AddProfile<UniLeagueProfile>();
        });
        var mapper = mapperConfig.CreateMapper();

        var service = new LeagueService(repo.Object, mapper);

        // Act
        var result = service.GetScheduleByLeague(999L);

        // Assert
        result.ShouldNotBeNull();
        result.Count.ShouldBe(0);
    }

    [Fact]
    public void Returns_matches_for_existing_league()
    {
        // Arrange
        var leagueId = 1L;
        var matches = new List<Solution.UniLeague.Core.Domain.Match>
        {
            CreateMatch(1, leagueId, 1, "Crvena zvezda", "Partizan"),
            CreateMatch(2, leagueId, 1, "Vojvodina", "Čukarički"),
            CreateMatch(3, leagueId, 2, "Partizan", "Vojvodina"),
            CreateMatch(4, leagueId, 2, "Čukarički", "Crvena zvezda")
        };

        var repo = new Mock<IMatchRepository>();
        repo.Setup(r => r.GetScheduleByLeague(leagueId))
            .Returns(new PagedResult<Solution.UniLeague.Core.Domain.Match>(matches, matches.Count));

        var mapperConfig = new MapperConfiguration(cfg =>
        {
            cfg.AddProfile<UniLeagueProfile>();
        });
        var mapper = mapperConfig.CreateMapper();

        var service = new LeagueService(repo.Object, mapper);

        // Act
        var result = service.GetScheduleByLeague(leagueId);

        // Assert
        result.ShouldNotBeNull();
        result.Count.ShouldBe(4);
        result.All(m => m.LeagueId == leagueId).ShouldBeTrue();
        result[0].HomeTeamName.ShouldBe("Crvena zvezda");
        result[0].AwayTeamName.ShouldBe("Partizan");
    }

    [Fact]
    public void Maps_all_match_properties_correctly()
    {
        // Arrange
        var leagueId = 1L;
        var scheduledAt = new DateTime(2025, 9, 14, 18, 0, 0);
        var match = CreateMatchDetailed(
            100, leagueId, 3,
            20, "Crvena zvezda", "/logos/zvezda.png",
            21, "Partizan", "/logos/partizan.png",
            scheduledAt);

        var repo = new Mock<IMatchRepository>();
        repo.Setup(r => r.GetScheduleByLeague(leagueId))
            .Returns(new PagedResult<Solution.UniLeague.Core.Domain.Match>(new List<Solution.UniLeague.Core.Domain.Match> { match }, 1));

        var mapperConfig = new MapperConfiguration(cfg =>
        {
            cfg.AddProfile<UniLeagueProfile>();
        });
        var mapper = mapperConfig.CreateMapper();

        var service = new LeagueService(repo.Object, mapper);

        // Act
        var result = service.GetScheduleByLeague(leagueId);

        // Assert
        result.Count.ShouldBe(1);
        var dto = result[0];
        dto.Id.ShouldBe(100);
        dto.LeagueId.ShouldBe(leagueId);
        dto.RoundNumber.ShouldBe(3);
        dto.HomeTeamId.ShouldBe(20);
        dto.HomeTeamName.ShouldBe("Crvena zvezda");
        dto.HomeTeamLogoUrl.ShouldBe("/logos/zvezda.png");
        dto.AwayTeamId.ShouldBe(21);
        dto.AwayTeamName.ShouldBe("Partizan");
        dto.AwayTeamLogoUrl.ShouldBe("/logos/partizan.png");
        dto.ScheduledAt.ShouldBe(scheduledAt);
    }

    [Fact]
    public void Filters_matches_by_league_id()
    {
        // Arrange
        var leagueId = 5L;
        var matches = new List<Solution.UniLeague.Core.Domain.Match>
        {
            CreateMatch(1, leagueId, 1, "Team A", "Team B"),
            CreateMatch(2, leagueId, 1, "Team C", "Team D")
        };

        var repo = new Mock<IMatchRepository>();
        repo.Setup(r => r.GetScheduleByLeague(leagueId))
            .Returns(new PagedResult<Solution.UniLeague.Core.Domain.Match>(matches, matches.Count));

        var mapperConfig = new MapperConfiguration(cfg =>
        {
            cfg.AddProfile<UniLeagueProfile>();
        });
        var mapper = mapperConfig.CreateMapper();

        var service = new LeagueService(repo.Object, mapper);

        // Act
        var result = service.GetScheduleByLeague(leagueId);

        // Assert
        result.ShouldNotBeNull();
        result.All(m => m.LeagueId == leagueId).ShouldBeTrue();
        repo.Verify(r => r.GetScheduleByLeague(leagueId), Times.Once);
    }

    // Helper metode
    private static Solution.UniLeague.Core.Domain.Match CreateMatch(long id, long leagueId, int roundNumber, string homeTeam, string awayTeam)
    {
        return CreateMatchDetailed(id, leagueId, roundNumber, 
            id * 10, homeTeam, "/logo.png", 
            id * 10 + 1, awayTeam, "/logo.png", 
            DateTime.Now);
    }

    private static Solution.UniLeague.Core.Domain.Match CreateMatchDetailed(
        long id, long leagueId, int roundNumber,
        long homeTeamId, string homeTeamName, string homeTeamLogoUrl,
        long awayTeamId, string awayTeamName, string awayTeamLogoUrl,
        DateTime scheduledAt)
    {
        var match = new Solution.UniLeague.Core.Domain.Match(
            leagueId, roundNumber,
            homeTeamId, homeTeamName, homeTeamLogoUrl,
            awayTeamId, awayTeamName, awayTeamLogoUrl,
            scheduledAt);

        
        var idProperty = typeof(Solution.UniLeague.Core.Domain.Match).BaseType.GetProperty("Id");
        idProperty?.SetValue(match, id);

        return match;
    }

    [Fact]
    public void GetResults_returns_only_matches_with_result()
    {
        // Arrange
        var leagueId = 1L;
        var matchWithResult = CreateMatch(1, leagueId, 1, "Crvena zvezda", "Partizan");
        matchWithResult.SetResult(MatchResult.Create(2, 1));

        var matchWithoutResult = CreateMatch(2, leagueId, 2, "Vojvodina", "Čukarički");

        var repo = new Mock<IMatchRepository>();
        repo.Setup(r => r.GetResultsByLeague(leagueId))
            .Returns(new PagedResult<Solution.UniLeague.Core.Domain.Match>(
                new List<Solution.UniLeague.Core.Domain.Match> { matchWithResult }, 1));

        var mapper = new MapperConfiguration(cfg => cfg.AddProfile<UniLeagueProfile>())
            .CreateMapper();
        var service = new LeagueService(repo.Object, mapper);

        // Act
        var result = service.GetResultsByLeague(leagueId);

        // Assert
        result.ShouldNotBeNull();
        result.Count.ShouldBe(1);
        result.All(m => m.Result != null).ShouldBeTrue();
    }

    [Fact]
    public void GetResults_maps_result_to_correct_format()
    {
        // Arrange
        var leagueId = 1L;
        var match = CreateMatch(1, leagueId, 1, "Crvena zvezda", "Partizan");
        match.SetResult(MatchResult.Create(3, 0));

        var repo = new Mock<IMatchRepository>();
        repo.Setup(r => r.GetResultsByLeague(leagueId))
            .Returns(new PagedResult<Solution.UniLeague.Core.Domain.Match>(
                new List<Solution.UniLeague.Core.Domain.Match> { match }, 1));

        var mapper = new MapperConfiguration(cfg => cfg.AddProfile<UniLeagueProfile>())
            .CreateMapper();
        var service = new LeagueService(repo.Object, mapper);

        // Act
        var result = service.GetResultsByLeague(leagueId);

        // Assert
        result[0].Result.ShouldBe("3:0");
    }

    [Fact]
    public void GetResults_returns_empty_list_when_no_results()
    {
        // Arrange
        var repo = new Mock<IMatchRepository>();
        repo.Setup(r => r.GetResultsByLeague(It.IsAny<long>()))
            .Returns(new PagedResult<Solution.UniLeague.Core.Domain.Match>(
                new List<Solution.UniLeague.Core.Domain.Match>(), 0));

        var mapper = new MapperConfiguration(cfg => cfg.AddProfile<UniLeagueProfile>())
            .CreateMapper();
        var service = new LeagueService(repo.Object, mapper);

        // Act
        var result = service.GetResultsByLeague(999L);

        // Assert
        result.ShouldNotBeNull();
        result.Count.ShouldBe(0);
    }
    [Fact]
    public void GetResults_maps_quarter_scores_when_present()
    {
        // Arrange
        var leagueId = 1L;
        var match = CreateMatch(1, leagueId, 1, "Crvena zvezda KK", "Partizan KK");

        // kreiramo MatchResult sa četvrtinama
        var quarters = new[]
        {
        QuarterScore.Create(1, 25, 21),
        QuarterScore.Create(2, 22, 25),
        QuarterScore.Create(3, 30, 28),
        QuarterScore.Create(4, 25, 21),
    };
        match.SetResult(MatchResult.CreateWithQuarters(102, 95, quarters));

        var repo = new Mock<IMatchRepository>();
        repo.Setup(r => r.GetResultsByLeague(leagueId))
            .Returns(new PagedResult<Solution.UniLeague.Core.Domain.Match>(new List<Solution.UniLeague.Core.Domain.Match> { match }, 1));

        var mapper = new MapperConfiguration(cfg => cfg.AddProfile<UniLeagueProfile>())
            .CreateMapper();
        var service = new LeagueService(repo.Object, mapper);

        // Act
        var result = service.GetResultsByLeague(leagueId);

        // Assert
        result[0].Result.ShouldBe("102:95");
        result[0].Quarters.ShouldNotBeNull();
        result[0].Quarters!.Count.ShouldBe(4);
        result[0].Quarters![0].QuarterNumber.ShouldBe(1);
        result[0].Quarters![0].HomeScore.ShouldBe(25);
        result[0].Quarters![0].AwayScore.ShouldBe(21);
    }

    [Fact]
    public void GetResults_returns_null_quarters_for_matches_without_quarters()
    {
        // Arrange
        var leagueId = 1L;
        var match = CreateMatch(1, leagueId, 1, "Partizan", "Vojvodina");
        match.SetResult(MatchResult.Create(2, 1)); // bez četvrtina

        var repo = new Mock<IMatchRepository>();
        repo.Setup(r => r.GetResultsByLeague(leagueId))
            .Returns(new PagedResult<Solution.UniLeague.Core.Domain.Match>(new List<Solution.UniLeague.Core.Domain.Match> { match }, 1));

        var mapper = new MapperConfiguration(cfg => cfg.AddProfile<UniLeagueProfile>())
            .CreateMapper();
        var service = new LeagueService(repo.Object, mapper);

        // Act
        var result = service.GetResultsByLeague(leagueId);

        // Assert
        result[0].Result.ShouldBe("2:1");
        result[0].Quarters.ShouldBeNull(); 
    }
}