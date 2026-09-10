namespace Solution.UniLeague.API.Dtos;

public class DelegateMatchDto
{
    public long Id { get; set; }
    public long LeagueId { get; set; }
    public string Sport { get; set; }
    public int RoundNumber { get; set; }
    public DateTime ScheduledAt { get; set; }
    public string Stage { get; set; } = "RegularSeason";
    public bool IsPlayoff { get; set; }

    public long HomeTeamId { get; set; }
    public string HomeTeamName { get; set; }
    public long AwayTeamId { get; set; }
    public string AwayTeamName { get; set; }

    public bool HasResult { get; set; }
    public string? Result { get; set; }

    public List<PlayerDto> HomeRoster { get; set; } = new();
    public List<PlayerDto> AwayRoster { get; set; } = new();

    public List<QuarterScoreDto>? Quarters { get; set; }
    public List<SetScoreDto>? Sets { get; set; }
    public List<GoalEventDto>? Goals { get; set; }
    public List<PlayerStatLineDto>? PlayerStats { get; set; }
}
