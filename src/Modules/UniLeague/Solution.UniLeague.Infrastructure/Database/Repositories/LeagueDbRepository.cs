using Microsoft.EntityFrameworkCore;
using Solution.UniLeague.Core.Domain;
using Solution.UniLeague.Core.RepositoryInterfaces;

namespace Solution.UniLeague.Infrastructure.Database.Repositories;

public class LeagueDbRepository : ILeagueRepository
{
    private readonly UniLeagueContext _context;

    public LeagueDbRepository(UniLeagueContext context)
    {
        _context = context;
    }

    public League? GetBySportAndGenderWithStandings(Sport sport, Gender? gender)
    {
        
        var query = _context.Leagues.Include(l => l.Standings)
            .Where(l => l.Sport == sport);

        if (sport == Sport.Volleyball)
            query = query.Where(l => l.LeagueGender == gender);

        return query.FirstOrDefault();
    }

    public League? GetByIdWithStandings(long leagueId)
    {
        return _context.Leagues
            .Include(l => l.Standings)
            .FirstOrDefault(l => l.Id == leagueId);
    }

    public List<League> GetAll()
    {
        return _context.Leagues.ToList();
    }

    public Sport? GetSportForTeam(int teamId, long excludingLeagueId)
    {
        return _context.StandingEntries
            .Where(s => s.TeamId == teamId && s.LeagueId != excludingLeagueId)
            .Join(_context.Leagues, s => s.LeagueId, l => l.Id, (s, l) => (Sport?)l.Sport)
            .FirstOrDefault();
    }
}
