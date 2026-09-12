namespace Solution.UniLeague.API.Dtos;

public class AdminMatchDto
{
    public long Id { get; set; }
    public long LeagueId { get; set; }
    public int RoundNumber { get; set; }
    public string Stage { get; set; } = "RegularSeason";
    public long HomeTeamId { get; set; }
    public string HomeTeamName { get; set; }
    public long AwayTeamId { get; set; }
    public string AwayTeamName { get; set; }
    public DateTime ScheduledAt { get; set; }
    public bool HasResult { get; set; }
    public string? Result { get; set; }
}
