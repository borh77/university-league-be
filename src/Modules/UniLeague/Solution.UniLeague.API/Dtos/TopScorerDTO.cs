namespace Solution.UniLeague.API.Dtos;

public class TopScorerDto
{
    public int Position { get; set; }
    public string ScorerName { get; set; }
    public string TeamName { get; set; }
    public string TeamLogoUrl { get; set; }
    public int Goals { get; set; }
}