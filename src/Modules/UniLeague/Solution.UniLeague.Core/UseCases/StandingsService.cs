using AutoMapper;
using Solution.BuildingBlocks.Core.Exceptions;
using Solution.UniLeague.API.Dtos;
using Solution.UniLeague.API.Public;
using Solution.UniLeague.Core.Domain;
using Solution.UniLeague.Core.Domain.RepositoryInterfaces;
using Solution.UniLeague.Core.RepositoryInterfaces;

namespace Solution.UniLeague.Core.UseCases;

public class StandingsService : IStandingsService
{
    private readonly ILeagueRepository _leagueRepository;
    private readonly IMatchRepository _matchRepository;
    private readonly IMapper _mapper;

    public StandingsService(
        ILeagueRepository leagueRepository,
        IMatchRepository matchRepository,
        IMapper mapper)
    {
        _leagueRepository = leagueRepository;
        _matchRepository = matchRepository;
        _mapper = mapper;
    }

    public List<StandingsRowDto> GetStandings(string sport, string? gender)
    {
        if (!Enum.TryParse<Sport>(sport, ignoreCase: true, out var parsedSport))
            throw new ArgumentException(
                $"Unknown sport: '{sport}'. Valid values: Football, Basketball, Volleyball.");

        Gender? parsedGender = null;
        if (parsedSport == Sport.Volleyball)
        {
            if (string.IsNullOrWhiteSpace(gender) ||
                !Enum.TryParse<Gender>(gender, ignoreCase: true, out var g))
            {
                throw new ArgumentException("For Volleyball, gender must be 'male' or 'female'.");
            }

            parsedGender = g;
        }

        var league = _leagueRepository.GetBySportAndGenderWithStandings(parsedSport, parsedGender)
            ?? throw new NotFoundException(
                $"League not found for sport '{parsedSport}'" +
                (parsedGender.HasValue ? $" and gender '{parsedGender}'" : string.Empty));

        var matches = _matchRepository.GetAllPlayedByLeague(league.Id);

        var sorter = new StandingsSorter();
        var sorted = sorter.Sort(league.Sport, league.Standings, matches);

        var rows = _mapper.Map<List<StandingsRowDto>>(sorted);

        for (var i = 0; i < rows.Count; i++)
        {
            rows[i].Position = i + 1;
        }

        return rows;
    }
}