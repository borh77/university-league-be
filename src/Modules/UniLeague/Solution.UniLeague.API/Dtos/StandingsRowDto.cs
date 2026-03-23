namespace Solution.UniLeague.API.Dtos;

public class StandingsRowDto
{
    public int Position { get; set; }
    public int TeamId { get; set; }
    public string TeamName { get; set; } = string.Empty;
    public string? LogoUrl { get; set; }
    public int Played { get; set; }
    public int Won { get; set; }
    public int Drawn { get; set; }
    public int Lost { get; set; }
    public int Points { get; set; }
    public int Scored { get; set; }
    public int Conceded { get; set; }
    public int Difference { get; set; }

    // Volleyball-specific treba izmeniti u dogovoru sa kolegom i klijentom da cela logika odbojke bude premestena u 
    // scored i conceded kao i u drugim sportovima jer od nje zavisi rangiranje
    // a poeni po setu sluze samo u prikazu rezultata meca
    public int? SetWon { get; set; }
    public int? SetLost { get; set; }
    public int? SetDifference { get; set; }
}