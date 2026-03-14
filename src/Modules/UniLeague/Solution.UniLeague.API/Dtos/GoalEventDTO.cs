namespace Solution.UniLeague.API.Dtos;

public class GoalEventDto
{
    public string ScorerName { get; set; }
    public string TeamName { get; set; }
    public bool IsHomeTeamGoal { get; set; }
    public int Minute { get; set; }
}