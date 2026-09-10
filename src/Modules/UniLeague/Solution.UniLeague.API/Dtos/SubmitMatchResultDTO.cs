namespace Solution.UniLeague.API.Dtos;

public class SubmitMatchResultDto
{
    public int HomeScore { get; set; }
    public int AwayScore { get; set; }

    // Opciona polja - servis bira odgovarajuću fabriku po League.Sport
    public List<QuarterScoreDto>? Quarters { get; set; }
    public List<SetScoreDto>? Sets { get; set; }
    public List<GoalEventDto>? Goals { get; set; }
    public List<PlayerStatInputDto>? PlayerStats { get; set; }
}
