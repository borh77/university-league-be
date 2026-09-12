using Solution.BuildingBlocks.Core.Exceptions;
using Solution.UniLeague.API.Dtos;
using Solution.UniLeague.API.Public;
using Solution.UniLeague.Core.Domain;
using Solution.UniLeague.Core.Domain.RepositoryInterfaces;
using Solution.UniLeague.Core.RepositoryInterfaces;

namespace Solution.UniLeague.Core.UseCases;

public class AdminLeagueService : IAdminLeagueService
{
    private readonly ILeagueRepository _leagueRepository;
    private readonly ITeamRepository _teamRepository;
    private readonly IStandingsRepository _standingsRepository;

    public AdminLeagueService(
        ILeagueRepository leagueRepository,
        ITeamRepository teamRepository,
        IStandingsRepository standingsRepository)
    {
        _leagueRepository = leagueRepository;
        _teamRepository = teamRepository;
        _standingsRepository = standingsRepository;
    }

    public List<AdminLeagueDto> GetAllLeagues()
    {
        return _leagueRepository.GetAll()
            .Select(l => new AdminLeagueDto
            {
                Id = l.Id,
                Sport = l.Sport.ToString(),
                Gender = l.LeagueGender?.ToString()
            })
            .ToList();
    }

    public List<AdminTeamDto> GetTeamsInLeague(long leagueId)
    {
        var league = _leagueRepository.GetByIdWithStandings(leagueId)
            ?? throw new NotFoundException($"League with id {leagueId} was not found.");

        return league.Standings
            .Select(s => new AdminTeamDto { Id = s.TeamId, Name = s.TeamName, LogoUrl = s.LogoUrl })
            .ToList();
    }

    public List<AdminTeamDto> GetAllTeams()
    {
        return _teamRepository.GetAll()
            .Select(t => new AdminTeamDto { Id = t.Id, Name = t.Name, LogoUrl = t.LogoUrl })
            .ToList();
    }

    public void AddTeamToLeague(long leagueId, long teamId)
    {
        var league = _leagueRepository.GetByIdWithStandings(leagueId)
            ?? throw new NotFoundException($"League with id {leagueId} was not found.");

        // Idempotentno - tim koji vec ima red u tabeli se ne dira
        if (league.Standings.Any(s => s.TeamId == (int)teamId))
            return;

        var team = _teamRepository.GetByIdWithPlayers(teamId);

        _standingsRepository.Add(new StandingEntry(
            leagueId, (int)teamId, team.Name, team.LogoUrl,
            0, 0, 0, 0, 0, 0, 0,
            league.Sport == Sport.Volleyball ? 0 : null,
            league.Sport == Sport.Volleyball ? 0 : null));
    }
}
