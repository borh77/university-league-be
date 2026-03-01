namespace Solution.UniLeague.API.Dtos;

public class TeamProfileDto
{
    public long Id { get; set; }
    public string Name { get; set; }
    public string? LogoUrl { get; set; }
    public List<PlayerDto> Players { get; set; } = new();
}