namespace Solution.UniLeague.API.Dtos;

public class PlayerStatLineDto
{
    public long PlayerId { get; set; }
    public string PlayerName { get; set; }
    public int JerseyNumber { get; set; }
    public bool IsHomeTeam { get; set; }
    public int Points { get; set; }
}
