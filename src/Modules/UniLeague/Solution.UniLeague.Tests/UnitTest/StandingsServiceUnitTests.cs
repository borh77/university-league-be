using AutoMapper;
using Moq;
using Shouldly;
using Solution.BuildingBlocks.Core.Exceptions;
using Solution.UniLeague.API.Dtos;
using Solution.UniLeague.Core.Domain;
using Solution.UniLeague.Core.Domain.RepositoryInterfaces;
using Solution.UniLeague.Core.Mappers;
using Solution.UniLeague.Core.RepositoryInterfaces;
using Solution.UniLeague.Core.UseCases;
using Match = Solution.UniLeague.Core.Domain.Match;

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
        var service = CreateService(Sport.Football, null, league);

        var result = service.GetStandings("football", null);

        result.ShouldNotBeNull();
        result.Count.ShouldBe(2);
    }

    [Fact]
    public void Positions_are_sequential_starting_from_one()
    {
        var league = CreateFootballLeagueWithEntries();
        var service = CreateService(Sport.Football, null, league);

        var result = service.GetStandings("football", null);

        for (var i = 0; i < result.Count; i++)
            result[i].Position.ShouldBe(i + 1);
    }

    [Fact]
    public void Returns_standings_for_volleyball_male()
    {
        var league = CreateVolleyballLeague(Gender.Male);
        var service = CreateService(Sport.Volleyball, Gender.Male, league);

        var result = service.GetStandings("volleyball", "male");

        result.ShouldNotBeNull();
        result.Count.ShouldBeGreaterThan(0);
    }

    [Fact]
    public void Returns_standings_for_volleyball_female()
    {
        var league = CreateVolleyballLeague(Gender.Female);
        var service = CreateService(Sport.Volleyball, Gender.Female, league);

        var result = service.GetStandings("volleyball", "female");

        result.ShouldNotBeNull();
        result.Count.ShouldBeGreaterThan(0);
    }

    [Fact]
    public void Sport_string_is_case_insensitive()
    {
        var league = CreateFootballLeagueWithEntries();
        var service = CreateService(Sport.Football, null, league);

        var result = service.GetStandings("FOOTBALL", null);

        result.ShouldNotBeNull();
    }

    [Fact]
    public void Gender_param_is_ignored_for_non_volleyball()
    {
        var league = CreateFootballLeagueWithEntries();
        var service = CreateService(Sport.Football, null, league);

        var withGender = service.GetStandings("football", "male");
        var withoutGender = service.GetStandings("football", null);

        withGender.Count.ShouldBe(withoutGender.Count);
    }

    [Fact]
    public void Maps_all_fields_correctly()
    {
        var league = CreateFootballLeagueWithEntries();
        var service = CreateService(Sport.Football, null, league);

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
        var service = CreateService(Sport.Football, null, CreateFootballLeagueWithEntries());

        var ex = Should.Throw<ArgumentException>(() => service.GetStandings("chess", null));
        ex.Message.ShouldContain("chess");
    }

    [Fact]
    public void Throws_argument_exception_for_volleyball_without_gender()
    {
        var service = CreateService(Sport.Football, null, CreateFootballLeagueWithEntries());

        Should.Throw<ArgumentException>(() => service.GetStandings("volleyball", null));
    }

    [Fact]
    public void Throws_argument_exception_for_volleyball_with_invalid_gender()
    {
        var service = CreateService(Sport.Football, null, CreateFootballLeagueWithEntries());

        Should.Throw<ArgumentException>(() => service.GetStandings("volleyball", "mixed"));
    }

    [Fact]
    public void Throws_not_found_exception_when_league_does_not_exist()
    {
        var leagueRepo = new Mock<ILeagueRepository>();
        leagueRepo.Setup(r => r.GetBySportAndGenderWithStandings(
                It.IsAny<Sport>(), It.IsAny<Gender?>()))
            .Returns((League?)null);

        var service = new StandingsService(
            leagueRepo.Object,
            new Mock<IMatchRepository>().Object,
            _mapper);

        Should.Throw<NotFoundException>(() => service.GetStandings("football", null));
    }

    // ── Helpers ──────────────────────────────────────────────────────

    private StandingsService CreateService(Sport sport, Gender? gender, League league,
        List<Match>? matches = null)
    {
        var leagueRepo = new Mock<ILeagueRepository>();
        leagueRepo.Setup(r => r.GetBySportAndGenderWithStandings(sport, gender))
            .Returns(league);

        var matchRepo = new Mock<IMatchRepository>();
        matchRepo.Setup(r => r.GetAllPlayedByLeague(It.IsAny<long>()))
            .Returns(matches ?? new List<Match>());

        return new StandingsService(leagueRepo.Object, matchRepo.Object, _mapper);
    }

    private static League CreateFootballLeagueWithEntries()
    {
        var league = new League(Sport.Football);
        GetStandingsField(league).AddRange(new[]
        {
            new StandingEntry(-1, 1, "Red Lions", "/logos/zvezda.png",  10, 7, 3, 0, 24, 22, 9),
            new StandingEntry(-1, 2, "Blue Eagles", "/logos/zvezda.png",10, 6, 3, 1, 21, 18, 11)
        });
        return league;
    }

    private static League CreateVolleyballLeague(Gender gender)
    {
        var league = new League(Sport.Volleyball, gender);
        GetStandingsField(league).AddRange(new[]
        {
            new StandingEntry(-2, 10, "Ace Spikers", "/logos/zvezda.png",   8, 7, 0, 1, 21, 890, 710, setWon: 21, setLost: 6),
            new StandingEntry(-2, 11, "Block Masters", "/logos/zvezda.png", 8, 5, 0, 3, 15, 820, 790, setWon: 17, setLost: 11)
        });
        return league;
    }

    private static List<StandingEntry> GetStandingsField(League league)
    {
        var field = typeof(League).GetField("_standings",
            System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance);
        return (List<StandingEntry>)field!.GetValue(league)!;
    }
}