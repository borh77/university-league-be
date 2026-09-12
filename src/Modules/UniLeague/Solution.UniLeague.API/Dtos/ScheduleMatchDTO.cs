namespace Solution.UniLeague.API.Dtos;

public class ScheduleMatchDto
{
    public int RoundNumber { get; set; }
    public long HomeTeamId { get; set; }
    public long AwayTeamId { get; set; }
    public DateTime ScheduledAt { get; set; }
}
