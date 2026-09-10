namespace Solution.UniLeague.API.Dtos;

public class PlayerStatInputDto
{
    public long PlayerId { get; set; }
    public bool IsHomeTeam { get; set; }
    public int Points { get; set; }
}
