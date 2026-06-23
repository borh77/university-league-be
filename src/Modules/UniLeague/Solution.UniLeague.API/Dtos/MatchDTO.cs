namespace Solution.UniLeague.API.Dtos;

public class MatchDto
{
    public long Id { get; set; }
    public long LeagueId { get; set; }
    public int RoundNumber { get; set; }
    public long HomeTeamId { get; set; }
    public string HomeTeamName { get; set; }
    public string HomeTeamLogoUrl { get; set; }
    public long AwayTeamId { get; set; }
    public string AwayTeamName { get; set; }
    public string AwayTeamLogoUrl { get; set; }
    public DateTime ScheduledAt { get; set; }
    public string? Result { get; set; }
    public string Stage { get; set; } = "RegularSeason";
    public bool IsPlayoff { get; set; }
    public string? PlayoffRoundLabel { get; set; }
    public int? HomeSeed { get; set; }
    public int? AwaySeed { get; set; }
    public List<QuarterScoreDto>? Quarters { get; set; }
    public List<SetScoreDto>? Sets { get; set; }
    public List<GoalEventDto>? Goals { get; set; }
}
