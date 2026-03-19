using Solution.UniLeague.Core.Domain;


namespace Solution.UniLeague.Tests.UnitTest
{
  
public class GoalEventTests
    {
        [Fact]
        public void Creates_successfully()
        {
            var g = GoalEvent.Create("Natcho", "Partizan", true, 23);
            g.ScorerName.ShouldBe("Natcho");
            g.TeamName.ShouldBe("Partizan");
            g.IsHomeTeamGoal.ShouldBeTrue();
            g.Minute.ShouldBe(23);
        }

        [Fact]
        public void Trims_whitespace_from_names()
        {
            var g = GoalEvent.Create("  Natcho  ", "  Partizan  ", true, 23);
            g.ScorerName.ShouldBe("Natcho");
            g.TeamName.ShouldBe("Partizan");
        }

        [Fact]
        public void Fails_with_null_scorer_name()
        {
            Should.Throw<ArgumentException>(() => GoalEvent.Create(null!, "Partizan", true, 23));
        }

        [Fact]
        public void Fails_with_empty_scorer_name()
        {
            Should.Throw<ArgumentException>(() => GoalEvent.Create("", "Partizan", true, 23));
        }

        [Fact]
        public void Fails_with_whitespace_scorer_name()
        {
            Should.Throw<ArgumentException>(() => GoalEvent.Create("   ", "Partizan", true, 23));
        }

        [Fact]
        public void Fails_with_null_team_name()
        {
            Should.Throw<ArgumentException>(() => GoalEvent.Create("Natcho", null!, true, 23));
        }

        [Fact]
        public void Fails_with_zero_minute()
        {
            Should.Throw<ArgumentException>(() => GoalEvent.Create("Natcho", "Partizan", true, 0));
        }

        [Fact]
        public void Fails_with_negative_minute()
        {
            Should.Throw<ArgumentException>(() => GoalEvent.Create("Natcho", "Partizan", true, -1));
        }

        [Fact]
        public void Away_goal_has_correct_flag()
        {
            var g = GoalEvent.Create("Šljivić", "Vojvodina", false, 45);
            g.IsHomeTeamGoal.ShouldBeFalse();
        }

        [Fact]
        public void ToString_returns_formatted_string()
        {
            GoalEvent.Create("Natcho", "Partizan", true, 23).ToString()
                .ShouldBe("23' Natcho (Partizan)");
        }
    }
}
