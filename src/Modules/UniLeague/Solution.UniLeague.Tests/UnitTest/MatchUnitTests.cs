using Shouldly;
using Solution.UniLeague.Core.Domain;

namespace Solution.UniLeague.Tests.Unit.Domain;

public class MatchTests
{
    [Fact]
    public void Creates()
    {
        // Arrange & Act
        var match = new Match(
            1L,
            1,
            10L, "Crvena zvezda", "/logos/zvezda.png",
            11L, "Partizan", "/logos/partizan.png",
            new DateTime(2025, 9, 14, 18, 0, 0));

        // Assert
        match.LeagueId.ShouldBe(1L);
        match.RoundNumber.ShouldBe(1);
        match.HomeTeamId.ShouldBe(10L);
        match.HomeTeamName.ShouldBe("Crvena zvezda");
        match.HomeTeamLogoUrl.ShouldBe("/logos/zvezda.png");
        match.AwayTeamId.ShouldBe(11L);
        match.AwayTeamName.ShouldBe("Partizan");
        match.AwayTeamLogoUrl.ShouldBe("/logos/partizan.png");
        match.ScheduledAt.ShouldBe(new DateTime(2025, 9, 14, 18, 0, 0));
    }

    [Fact]
    public void Fails_with_invalid_league_id()
    {
        Should.Throw<ArgumentException>(() => new Match(
            0L,
            1,
            10L, "Crvena zvezda", "/logos/zvezda.png",
            11L, "Partizan", "/logos/partizan.png",
            DateTime.Now));
    }

    [Fact]
    public void Fails_with_negative_league_id()
    {
        Should.Throw<ArgumentException>(() => new Match(
            -1L,
            1,
            10L, "Crvena zvezda", "/logos/zvezda.png",
            11L, "Partizan", "/logos/partizan.png",
            DateTime.Now));
    }

    [Fact]
    public void Fails_with_invalid_round_number()
    {
        Should.Throw<ArgumentException>(() => new Match(
            1L,
            0,
            10L, "Crvena zvezda", "/logos/zvezda.png",
            11L, "Partizan", "/logos/partizan.png",
            DateTime.Now));
    }

    [Fact]
    public void Fails_with_negative_round_number()
    {
        Should.Throw<ArgumentException>(() => new Match(
            1L,
            -1,
            10L, "Crvena zvezda", "/logos/zvezda.png",
            11L, "Partizan", "/logos/partizan.png",
            DateTime.Now));
    }

    [Fact]
    public void Fails_with_null_home_team_name()
    {
        Should.Throw<ArgumentException>(() => new Match(
            1L,
            1,
            10L, null, "/logos/zvezda.png",
            11L, "Partizan", "/logos/partizan.png",
            DateTime.Now));
    }

    [Fact]
    public void Fails_with_empty_home_team_name()
    {
        Should.Throw<ArgumentException>(() => new Match(
            1L,
            1,
            10L, "", "/logos/zvezda.png",
            11L, "Partizan", "/logos/partizan.png",
            DateTime.Now));
    }

    [Fact]
    public void Fails_with_whitespace_home_team_name()
    {
        Should.Throw<ArgumentException>(() => new Match(
            1L,
            1,
            10L, "   ", "/logos/zvezda.png",
            11L, "Partizan", "/logos/partizan.png",
            DateTime.Now));
    }

    [Fact]
    public void Fails_with_null_away_team_name()
    {
        Should.Throw<ArgumentException>(() => new Match(
            1L,
            1,
            10L, "Crvena zvezda", "/logos/zvezda.png",
            11L, null, "/logos/partizan.png",
            DateTime.Now));
    }

    [Fact]
    public void Fails_with_empty_away_team_name()
    {
        Should.Throw<ArgumentException>(() => new Match(
            1L,
            1,
            10L, "Crvena zvezda", "/logos/zvezda.png",
            11L, "", "/logos/partizan.png",
            DateTime.Now));
    }

    [Fact]
    public void Fails_with_whitespace_away_team_name()
    {
        Should.Throw<ArgumentException>(() => new Match(
            1L,
            1,
            10L, "Crvena zvezda", "/logos/zvezda.png",
            11L, "   ", "/logos/partizan.png",
            DateTime.Now));
    }

    [Fact]
    public void Fails_when_home_and_away_teams_are_same()
    {
        Should.Throw<ArgumentException>(() => new Match(
            1L,
            1,
            10L, "Crvena zvezda", "/logos/zvezda.png",
            10L, "Partizan", "/logos/partizan.png",
            DateTime.Now));
    }

    [Fact]
    public void Creates_with_null_logo_urls()
    {
        // Arrange & Act
        var match = new Match(
            1L,
            1,
            10L, "Crvena zvezda", null,
            11L, "Partizan", null,
            DateTime.Now);

        // Assert
        match.HomeTeamLogoUrl.ShouldBeNull();
        match.AwayTeamLogoUrl.ShouldBeNull();
    }

    [Fact]
    public void Creates_with_same_team_names_but_different_ids()
    {
        // Arrange & Act
        var match = new Match(
            1L,
            1,
            10L, "Crvena zvezda", "/logos/zvezda.png",
            11L, "Crvena zvezda", "/logos/zvezda2.png",
            DateTime.Now);

        // Assert
        match.HomeTeamId.ShouldBe(10L);
        match.AwayTeamId.ShouldBe(11L);
        match.HomeTeamName.ShouldBe("Crvena zvezda");
        match.AwayTeamName.ShouldBe("Crvena zvezda");
    }
}