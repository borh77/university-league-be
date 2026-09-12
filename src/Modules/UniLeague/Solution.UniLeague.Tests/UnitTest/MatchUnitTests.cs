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
    public void Creates_with_negative_league_id()
    {
        var match = new Match(
            -1L,
            1,
            10L, "Crvena zvezda", "/logos/zvezda.png",
            11L, "Partizan", "/logos/partizan.png",
            DateTime.Now);

        match.LeagueId.ShouldBe(-1L);
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

    [Fact]
    public void Has_no_result_by_default()
    {
        var match = new Match(
            1L, 1,
            10L, "Crvena zvezda", "/logos/zvezda.png",
            11L, "Partizan", "/logos/partizan.png",
            DateTime.Now);

        match.HasResult.ShouldBeFalse();
        match.Result.ShouldBeNull();
    }

    [Fact]
    public void Sets_result_successfully()
    {
        var match = new Match(
            1L, 1,
            10L, "Crvena zvezda", "/logos/zvezda.png",
            11L, "Partizan", "/logos/partizan.png",
            DateTime.Now);

        match.SetResult(MatchResult.Create(2, 1));

        match.HasResult.ShouldBeTrue();
        match.Result!.HomeScore.ShouldBe(2);
        match.Result.AwayScore.ShouldBe(1);
    }

    [Fact]
    public void SetResult_rejects_null()
    {
        var match = new Match(
            1L, 1,
            10L, "Crvena zvezda", "/logos/zvezda.png",
            11L, "Partizan", "/logos/partizan.png",
            DateTime.Now);

        Should.Throw<ArgumentNullException>(() => match.SetResult(null!));
    }

    [Fact]
    public void Reschedule_changes_scheduled_at()
    {
        var match = new Match(
            1L, 1,
            10L, "Crvena zvezda", "/logos/zvezda.png",
            11L, "Partizan", "/logos/partizan.png",
            new DateTime(2026, 1, 1, 18, 0, 0));

        match.Reschedule(new DateTime(2026, 2, 1, 20, 0, 0));

        match.ScheduledAt.ShouldBe(new DateTime(2026, 2, 1, 20, 0, 0));
    }

    [Fact]
    public void Reschedule_works_for_playoff_match()
    {
        var match = new Match(
            1L, 1,
            10L, "Crvena zvezda", "/logos/zvezda.png",
            11L, "Partizan", "/logos/partizan.png",
            new DateTime(2026, 1, 1, 18, 0, 0),
            MatchStage.PlayoffFinal,
            1, 2);

        match.Reschedule(new DateTime(2026, 3, 1, 20, 0, 0));

        match.ScheduledAt.ShouldBe(new DateTime(2026, 3, 1, 20, 0, 0));
    }

    [Fact]
    public void ClearResult_removes_the_result()
    {
        var match = new Match(
            1L, 1,
            10L, "Crvena zvezda", "/logos/zvezda.png",
            11L, "Partizan", "/logos/partizan.png",
            DateTime.Now);
        match.SetResult(MatchResult.Create(2, 1));

        match.ClearResult();

        match.HasResult.ShouldBeFalse();
        match.Result.ShouldBeNull();
    }

    [Fact]
    public void SetTeams_changes_teams_when_match_has_no_result()
    {
        var match = new Match(
            1L, 1,
            10L, "Crvena zvezda", "/logos/zvezda.png",
            11L, "Partizan", "/logos/partizan.png",
            DateTime.Now);

        match.SetTeams(20L, "Vojvodina", "/logos/vojvodina.png", 21L, "Radnicki", "/logos/radnicki.png");

        match.HomeTeamId.ShouldBe(20L);
        match.HomeTeamName.ShouldBe("Vojvodina");
        match.HomeTeamLogoUrl.ShouldBe("/logos/vojvodina.png");
        match.AwayTeamId.ShouldBe(21L);
        match.AwayTeamName.ShouldBe("Radnicki");
        match.AwayTeamLogoUrl.ShouldBe("/logos/radnicki.png");
    }

    [Fact]
    public void SetTeams_throws_when_match_already_has_a_result()
    {
        var match = new Match(
            1L, 1,
            10L, "Crvena zvezda", "/logos/zvezda.png",
            11L, "Partizan", "/logos/partizan.png",
            DateTime.Now);
        match.SetResult(MatchResult.Create(2, 1));

        Should.Throw<ArgumentException>(() =>
            match.SetTeams(20L, "Vojvodina", "/logos/vojvodina.png", 21L, "Radnicki", "/logos/radnicki.png"));
    }

    [Fact]
    public void SetTeams_throws_for_playoff_match()
    {
        var match = new Match(
            1L, 1,
            10L, "Crvena zvezda", "/logos/zvezda.png",
            11L, "Partizan", "/logos/partizan.png",
            DateTime.Now,
            MatchStage.PlayoffSemifinal,
            1, 2);

        Should.Throw<ArgumentException>(() =>
            match.SetTeams(20L, "Vojvodina", "/logos/vojvodina.png", 21L, "Radnicki", "/logos/radnicki.png"));
    }

    [Fact]
    public void SetTeams_throws_when_home_and_away_are_the_same_team()
    {
        var match = new Match(
            1L, 1,
            10L, "Crvena zvezda", "/logos/zvezda.png",
            11L, "Partizan", "/logos/partizan.png",
            DateTime.Now);

        Should.Throw<ArgumentException>(() =>
            match.SetTeams(20L, "Vojvodina", "/logos/vojvodina.png", 20L, "Vojvodina", "/logos/vojvodina.png"));
    }
}
