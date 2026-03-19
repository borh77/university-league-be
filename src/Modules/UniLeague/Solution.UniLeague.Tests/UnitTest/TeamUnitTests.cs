using Shouldly;
using Solution.UniLeague.Core.Domain;

namespace Solution.UniLeague.Tests.Unit.Domain;

public class TeamUnitTests
{
    [Fact]
    public void Creates_successfully()
    {
        var team = new Team("Crvena zvezda", "/logos/zvezda.png");

        team.Name.ShouldBe("Crvena zvezda");
        team.LogoUrl.ShouldBe("/logos/zvezda.png");
        team.Players.ShouldBeEmpty();
    }

    [Fact]
    public void Creates_with_null_logo()
    {
        var team = new Team("Crvena zvezda", null);

        team.LogoUrl.ShouldBeNull();
    }

    [Fact]
    public void Fails_with_null_name()
    {
        Should.Throw<ArgumentException>(() =>
            new Team(null!, "/logos/zvezda.png"));
    }

    [Fact]
    public void Fails_with_empty_name()
    {
        Should.Throw<ArgumentException>(() =>
            new Team("", "/logos/zvezda.png"));
    }

    [Fact]
    public void Fails_with_whitespace_name()
    {
        Should.Throw<ArgumentException>(() =>
            new Team("   ", "/logos/zvezda.png"));
    }

    // ======================
    // Player testovi
    // ======================

    [Fact]
    public void Player_creates_successfully()
    {
        var player = new Player(10, "Marko", "Marković", 10, "/players/markovic.png");

        player.TeamId.ShouldBe(10);
        player.FirstName.ShouldBe("Marko");
        player.LastName.ShouldBe("Marković");
        player.JerseyNumber.ShouldBe(10);
        player.ImageUrl.ShouldBe("/players/markovic.png");
    }

    [Fact]
    public void Player_creates_with_null_image()
    {
        var player = new Player(10, "Marko", "Marković", 10, null);

        player.ImageUrl.ShouldBeNull();
    }

    [Fact]
    public void Player_fails_with_zero_team_id()
    {
        Should.Throw<ArgumentException>(() =>
            new Player(0, "Marko", "Marković", 10, null));
    }

    [Fact]
    public void Player_fails_with_negative_team_id()
    {
        Should.Throw<ArgumentException>(() =>
            new Player(-1, "Marko", "Marković", 10, null));
    }

    [Fact]
    public void Player_fails_with_null_first_name()
    {
        Should.Throw<ArgumentException>(() =>
            new Player(10, null!, "Marković", 10, null));
    }

    [Fact]
    public void Player_fails_with_empty_first_name()
    {
        Should.Throw<ArgumentException>(() =>
            new Player(10, "", "Marković", 10, null));
    }

    [Fact]
    public void Player_fails_with_whitespace_first_name()
    {
        Should.Throw<ArgumentException>(() =>
            new Player(10, "   ", "Marković", 10, null));
    }

    [Fact]
    public void Player_fails_with_null_last_name()
    {
        Should.Throw<ArgumentException>(() =>
            new Player(10, "Marko", null!, 10, null));
    }

    [Fact]
    public void Player_fails_with_empty_last_name()
    {
        Should.Throw<ArgumentException>(() =>
            new Player(10, "Marko", "", 10, null));
    }

    [Fact]
    public void Player_fails_with_whitespace_last_name()
    {
        Should.Throw<ArgumentException>(() =>
            new Player(10, "Marko", "   ", 10, null));
    }

    [Fact]
    public void Player_fails_with_jersey_number_zero()
    {
        Should.Throw<ArgumentException>(() =>
            new Player(10, "Marko", "Marković", 0, null));
    }

    [Fact]
    public void Player_fails_with_jersey_number_over_99()
    {
        Should.Throw<ArgumentException>(() =>
            new Player(10, "Marko", "Marković", 100, null));
    }
}