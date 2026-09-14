namespace Solution.UniLeague.API.Dtos;

public class AdminTeamDto
{
    public long Id { get; set; }
    public string Name { get; set; }
    public string? LogoUrl { get; set; }

    // Sport lige u kojoj tim vec igra, prazno ako tim nigde ne igra
    public string? Sport { get; set; }
}
