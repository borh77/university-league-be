using Solution.UniLeague.API.Dtos;

namespace Solution.UniLeague.API.Public;

public interface IStandingsService
{
    List<StandingsRowDto> GetStandings(string sport, string? gender);
}