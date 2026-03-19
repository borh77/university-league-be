namespace Solution.UniLeague.API.Dtos;

public class PlayerDto
{
    public long Id { get; set; }
    public int JerseyNumber { get; set; }
    public string FirstName { get; set; }
    public string LastName { get; set; }
    public string? ImageUrl { get; set; }
}