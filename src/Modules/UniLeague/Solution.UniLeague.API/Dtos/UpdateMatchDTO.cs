namespace Solution.UniLeague.API.Dtos;

public class UpdateMatchDto
{
    // Sva polja opciona - postavi se samo ono sto je poslato
    public DateTime? ScheduledAt { get; set; }
    public long? HomeTeamId { get; set; }
    public long? AwayTeamId { get; set; }
}
