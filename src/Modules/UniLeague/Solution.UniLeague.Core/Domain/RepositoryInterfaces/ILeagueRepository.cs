using Solution.UniLeague.Core.Domain;

namespace Solution.UniLeague.Core.RepositoryInterfaces;

public interface ILeagueRepository
{
    /// <summary>
    /// Za Volleyball: filtrira i po sport i po gender.
    /// Za ostale sportove: filtrira samo po sport (gender se ignoriše).
    /// </summary>
    League? GetBySportAndGenderWithStandings(Sport sport, Gender? gender);
    League? GetByIdWithStandings(long leagueId);
    List<League> GetAll();

    // Sport lige u kojoj tim vec ima red u tabeli (izuzev prosledjene lige), null ako ga nema nigde
    Sport? GetSportForTeam(int teamId, long excludingLeagueId);
}
