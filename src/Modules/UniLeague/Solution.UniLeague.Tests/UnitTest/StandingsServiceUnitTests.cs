using AutoMapper;
using Moq;
using Shouldly;
using Solution.BuildingBlocks.Core.Exceptions;
using Solution.UniLeague.API.Dtos;
using Solution.UniLeague.Core.Domain;
using Solution.UniLeague.Core.Mappers;
using Solution.UniLeague.Core.RepositoryInterfaces;
using Solution.UniLeague.Core.UseCases;
using Xunit;

namespace Solution.UniLeague.Tests.Unit;

public class StandingsServiceUnitTests
{
    private readonly IMapper _mapper;

    public StandingsServiceUnitTests()
    {
        _mapper = new MapperConfiguration(cfg => cfg.AddProfile<UniLeagueProfile>())
            .CreateMapper();
    }

    
    [Fact]
    public void Returns_standings_for_football()
    {
        var league = CreateFootballLeagueWithEntries();
        var repo = MockRepo(Sport.Football, null, league);
        var service = new StandingsService(repo.Object, _mapper);

        var result = service.GetStandings("football", null);

        result.ShouldNotBeNull();
        result.Count.ShouldBe(2);
    }

    [Fact]
    public void Positions_are_sequential_starting_from_one()
    {
        var league = CreateFootballLeagueWithEntries();
        var repo = MockRepo(Sport.Football, null, league);
        var service = new StandingsService(repo.Object, _mapper);

        var result = service.GetStandings("football", null);

        for (var i = 0; i < result.Count; i++)
            result[i].Position.ShouldBe(i + 1);
    }

    [Fact]
    public void Returns_standings_for_volleyball_male()
    {
        var league = CreateVolleyballLeague(Gender.Male);
        var repo = MockRepo(Sport.Volleyball, Gender.Male, league);
        var service = new StandingsService(repo.Object, _mapper);

        var result = service.GetStandings("volleyball", "male");

        result.ShouldNotBeNull();
        result.Count.ShouldBeGreaterThan(0);
    }

    [Fact]
    public void Returns_standings_for_volleyball_female()
    {
        var league = CreateVolleyballLeague(Gender.Female);
        var repo = MockRepo(Sport.Volleyball, Gender.Female, league);
        var service = new StandingsService(repo.Object, _mapper);

        var result = service.GetStandings("volleyball", "female");

        result.ShouldNotBeNull();
        result.Count.ShouldBeGreaterThan(0);
    }

    [Fact]
    public void Sport_string_is_case_insensitive()
    {
        var league = CreateFootballLeagueWithEntries();
        var repo = MockRepo(Sport.Football, null, league);
        var service = new StandingsService(repo.Object, _mapper);

        var result = service.GetStandings("FOOTBALL", null);

        result.ShouldNotBeNull();
    }

    [Fact]
    public void Gender_param_is_ignored_for_non_volleyball()
    {
        var league = CreateFootballLeagueWithEntries();
        var repo = MockRepo(Sport.Football, null, league);
        var service = new StandingsService(repo.Object, _mapper);

        var withGender = service.GetStandings("football", "male");
        var withoutGender = service.GetStandings("football", null);

        withGender.Count.ShouldBe(withoutGender.Count);
    }

    [Fact]
    public void Maps_all_fields_correctly()
    {
        var league = CreateFootballLeagueWithEntries();
        var repo = MockRepo(Sport.Football, null, league);
        var service = new StandingsService(repo.Object, _mapper);

        var result = service.GetStandings("football", null);

        var first = result[0];
        first.TeamId.ShouldNotBe(0);
        first.TeamName.ShouldNotBeNullOrWhiteSpace();
        first.Points.ShouldBeGreaterThanOrEqualTo(0);
        first.Difference.ShouldBe(first.Scored - first.Conceded);
    }

    [Fact]
    public void Throws_argument_exception_for_unknown_sport()
    {
        var repo = new Mock<ILeagueRepository>();
        var service = new StandingsService(repo.Object, _mapper);

        var ex = Should.Throw<ArgumentException>(() => service.GetStandings("chess", null));
        ex.Message.ShouldContain("chess");
    }

    [Fact]
    public void Throws_argument_exception_for_volleyball_without_gender()
    {
        var repo = new Mock<ILeagueRepository>();
        var service = new StandingsService(repo.Object, _mapper);

        Should.Throw<ArgumentException>(() => service.GetStandings("volleyball", null));
    }

    [Fact]
    public void Throws_argument_exception_for_volleyball_with_invalid_gender()
    {
        var repo = new Mock<ILeagueRepository>();
        var service = new StandingsService(repo.Object, _mapper);

        Should.Throw<ArgumentException>(() => service.GetStandings("volleyball", "mixed"));
    }

    [Fact]
    public void Throws_not_found_exception_when_league_does_not_exist()
    {
        var repo = new Mock<ILeagueRepository>();
        repo.Setup(r => r.GetBySportAndGenderWithStandings(It.IsAny<Sport>(), It.IsAny<Gender?>()))
            .Returns((League?)null);
        var service = new StandingsService(repo.Object, _mapper);

        Should.Throw<NotFoundException>(() => service.GetStandings("football", null));
    }

   
    private static Mock<ILeagueRepository> MockRepo(Sport sport, Gender? gender, League league)
    {
        var repo = new Mock<ILeagueRepository>();
        repo.Setup(r => r.GetBySportAndGenderWithStandings(sport, gender))
            .Returns(league);
        return repo;
    }

    private static League CreateFootballLeagueWithEntries()
    {
        var league = new League(Sport.Football);
        var standings = GetStandingsField(league);
        standings.Add(new StandingEntry(-1, 1, "Red Lions", 10, 7, 3, 0, 24, 22, 9));
        standings.Add(new StandingEntry(-1, 2, "Blue Eagles", 10, 6, 3, 1, 21, 18, 11));
        return league;
    }

    private static League CreateVolleyballLeague(Gender gender)
    {
        var league = new League(Sport.Volleyball, gender);
        var standings = GetStandingsField(league);
        standings.Add(new StandingEntry(-2, 10, "Ace Spikers", 8, 7, 0, 1, 21, 890, 710, setWon: 21, setLost: 6));
        standings.Add(new StandingEntry(-2, 11, "Block Masters", 8, 5, 0, 3, 15, 820, 790, setWon: 17, setLost: 11));
        return league;
    }

    private static List<StandingEntry> GetStandingsField(League league)
    {
        var field = typeof(League)
            .GetField("_standings", System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance);
        return (List<StandingEntry>)field!.GetValue(league)!;
    }
}