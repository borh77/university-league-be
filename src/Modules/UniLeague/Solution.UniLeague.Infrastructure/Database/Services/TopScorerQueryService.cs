using Solution.UniLeague.API.Dtos;
using Solution.UniLeague.Core.UseCases;
using Solution.UniLeague.Infrastructure.Database;

namespace Solution.UniLeague.Infrastructure.Database.Services;

public class TopScorerQueryService : ITopScorerQueryService
{
    private readonly UniLeagueContext _context;

    public TopScorerQueryService(UniLeagueContext context)
    {
        _context = context;
    }

    public List<TopScorerDto> GetTopScorersByLeague(long leagueId)
    {
        return _context.Matches
            .Where(m => m.LeagueId == leagueId && m.Result != null)
            .SelectMany(m => m.Result!.Goals.Select(g => new
            {
                g.ScorerName,
                g.TeamName,
                TeamLogoUrl = g.IsHomeTeamGoal ? m.HomeTeamLogoUrl : m.AwayTeamLogoUrl,
                MatchId = m.Id
            }))
            .GroupBy(g => new { g.ScorerName, g.TeamName, g.TeamLogoUrl })
            .Select(grp => new TopScorerDto
            {
                ScorerName = grp.Key.ScorerName,
                TeamName = grp.Key.TeamName,
                TeamLogoUrl = grp.Key.TeamLogoUrl,
                Goals = grp.Count(),
                
            })
            .OrderByDescending(s => s.Goals)
            .ThenBy(s => s.ScorerName)
            .ToList();
    }
}