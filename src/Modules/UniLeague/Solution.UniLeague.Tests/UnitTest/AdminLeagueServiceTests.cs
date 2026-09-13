using Moq;
using Shouldly;
using Solution.UniLeague.API.Dtos;
using Solution.UniLeague.Core.Domain;
using Solution.UniLeague.Core.Domain.RepositoryInterfaces;
using Solution.UniLeague.Core.RepositoryInterfaces;
using Solution.UniLeague.Core.UseCases;

namespace Solution.UniLeague.Tests.UnitTest;

public class AdminLeagueServiceTests
{
    private const long LeagueId = 1;
    private const long TeamId = 10;

    [Fact]
    public void AddTeamToLeague_rejects_team_that_already_plays_a_different_sport()
    {
        var footballLeague = new League(Sport.Football);
        var basketballLeague = new League(Sport.Basketball);

        var leagueRepo = new Mock<ILeagueRepository>();
        leagueRepo.Setup(r => r.GetByIdWithStandings(LeagueId)).Returns(basketballLeague);
        leagueRepo.Setup(r => r.GetSportForTeam((int)TeamId, LeagueId)).Returns(footballLeague.Sport);

        var service = new AdminLeagueService(
            leagueRepo.Object,
            new Mock<ITeamRepository>().Object,
            new Mock<IStandingsRepository>().Object);

        Should.Throw<ArgumentException>(() => service.AddTeamToLeague(LeagueId, TeamId));
    }

    [Fact]
    public void AddTeamToLeague_allows_team_without_existing_league()
    {
        var league = new League(Sport.Football);
        var team = new Team("Red Lions FC", "/logos/red-lions.png");

        var leagueRepo = new Mock<ILeagueRepository>();
        leagueRepo.Setup(r => r.GetByIdWithStandings(LeagueId)).Returns(league);
        leagueRepo.Setup(r => r.GetSportForTeam((int)TeamId, LeagueId)).Returns((Sport?)null);

        var teamRepo = new Mock<ITeamRepository>();
        teamRepo.Setup(r => r.GetByIdWithPlayers(TeamId)).Returns(team);

        var standingsRepo = new Mock<IStandingsRepository>();

        var service = new AdminLeagueService(leagueRepo.Object, teamRepo.Object, standingsRepo.Object);

        service.AddTeamToLeague(LeagueId, TeamId);

        standingsRepo.Verify(r => r.Add(It.Is<StandingEntry>(e => e.TeamId == (int)TeamId)), Times.Once);
    }

    [Fact]
    public void AddTeamToLeague_allows_team_already_in_another_league_of_the_same_sport()
    {
        var footballLeagueA = new League(Sport.Football);
        var team = new Team("Blue Eagles", "/logos/blue-eagles.png");

        var leagueRepo = new Mock<ILeagueRepository>();
        leagueRepo.Setup(r => r.GetByIdWithStandings(LeagueId)).Returns(footballLeagueA);
        leagueRepo.Setup(r => r.GetSportForTeam((int)TeamId, LeagueId)).Returns(Sport.Football);

        var teamRepo = new Mock<ITeamRepository>();
        teamRepo.Setup(r => r.GetByIdWithPlayers(TeamId)).Returns(team);

        var standingsRepo = new Mock<IStandingsRepository>();

        var service = new AdminLeagueService(leagueRepo.Object, teamRepo.Object, standingsRepo.Object);

        service.AddTeamToLeague(LeagueId, TeamId);

        standingsRepo.Verify(r => r.Add(It.IsAny<StandingEntry>()), Times.Once);
    }
}
