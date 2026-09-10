using Solution.BuildingBlocks.Core.Exceptions;
using Solution.UniLeague.Core.Domain;
using Solution.UniLeague.Core.Domain.RepositoryInterfaces;
using Solution.UniLeague.Core.RepositoryInterfaces;

namespace Solution.UniLeague.Core.UseCases;

public class StandingsRecalculationService : IStandingsRecalculationService
{
    private readonly ILeagueRepository _leagueRepository;
    private readonly IMatchRepository _matchRepository;
    private readonly IStandingsRepository _standingsRepository;

    public StandingsRecalculationService(
        ILeagueRepository leagueRepository,
        IMatchRepository matchRepository,
        IStandingsRepository standingsRepository)
    {
        _leagueRepository = leagueRepository;
        _matchRepository = matchRepository;
        _standingsRepository = standingsRepository;
    }

    public void Recalculate(long leagueId)
    {
        var league = _leagueRepository.GetByIdWithStandings(leagueId)
            ?? throw new NotFoundException($"League with id {leagueId} was not found.");

        // GetAllPlayedByLeague vraća samo regularne mečeve sa rezultatom - plej-of se ne dira
        var matches = _matchRepository.GetAllPlayedByLeague(leagueId);

        var rows = new Dictionary<long, Row>();

        // Spisak timova u ligi je polazna tačka - tim koji još nije igrao ostaje u tabeli sa nulama
        foreach (var team in league.Standings)
            rows[team.TeamId] = new Row(team.TeamId, team.TeamName, team.LogoUrl);

        Row RowFor(long teamId, string teamName, string? logoUrl)
        {
            if (!rows.TryGetValue(teamId, out var row))
            {
                row = new Row(teamId, teamName, logoUrl);
                rows[teamId] = row;
            }
            return row;
        }

        foreach (var match in matches)
        {
            var home = RowFor(match.HomeTeamId, match.HomeTeamName, match.HomeTeamLogoUrl);
            var away = RowFor(match.AwayTeamId, match.AwayTeamName, match.AwayTeamLogoUrl);
            Apply(league.Sport, home, away, match.Result!);
        }

        var entries = rows.Values
            .Select(r => r.ToEntry(leagueId, league.Sport))
            .ToList();

        _standingsRepository.ReplaceForLeague(leagueId, entries);
    }

    private static void Apply(Sport sport, Row home, Row away, MatchResult result)
    {
        home.Played++;
        away.Played++;

        switch (sport)
        {
            case Sport.Volleyball:
                ApplyVolleyball(home, away, result);
                break;
            case Sport.Basketball:
                ApplyBasketball(home, away, result);
                break;
            default:
                ApplyFootball(home, away, result);
                break;
        }
    }

    // Fudbal: 3 / 1 / 0
    private static void ApplyFootball(Row home, Row away, MatchResult result)
    {
        home.Scored += result.HomeScore;
        home.Conceded += result.AwayScore;
        away.Scored += result.AwayScore;
        away.Conceded += result.HomeScore;

        if (result.IsDraw())
        {
            home.Drawn++;
            away.Drawn++;
            home.Points += 1;
            away.Points += 1;
        }
        else if (result.HomeWon())
        {
            home.Won++;
            away.Lost++;
            home.Points += 3;
        }
        else
        {
            away.Won++;
            home.Lost++;
            away.Points += 3;
        }
    }

    // Košarka: 2 (pobeda) / 1 (poraz), nema nerešeno
    private static void ApplyBasketball(Row home, Row away, MatchResult result)
    {
        home.Scored += result.HomeScore;
        home.Conceded += result.AwayScore;
        away.Scored += result.AwayScore;
        away.Conceded += result.HomeScore;

        if (result.HomeWon())
        {
            home.Won++;
            away.Lost++;
            home.Points += 2;
            away.Points += 1;
        }
        else
        {
            away.Won++;
            home.Lost++;
            away.Points += 2;
            home.Points += 1;
        }
    }

    // Odbojka: 3 (pobeda) / 0 (poraz); Scored/Conceded su poeni po setovima
    private static void ApplyVolleyball(Row home, Row away, MatchResult result)
    {
        home.SetWon += result.HomeScore;
        home.SetLost += result.AwayScore;
        away.SetWon += result.AwayScore;
        away.SetLost += result.HomeScore;

        foreach (var set in result.Sets)
        {
            home.Scored += set.HomeScore;
            home.Conceded += set.AwayScore;
            away.Scored += set.AwayScore;
            away.Conceded += set.HomeScore;
        }

        if (result.HomeWon())
        {
            home.Won++;
            away.Lost++;
            home.Points += 3;
        }
        else
        {
            away.Won++;
            home.Lost++;
            away.Points += 3;
        }
    }

    private sealed class Row
    {
        public long TeamId { get; }
        public string TeamName { get; }
        public string? LogoUrl { get; }
        public int Played;
        public int Won;
        public int Drawn;
        public int Lost;
        public int Points;
        public int Scored;
        public int Conceded;
        public int SetWon;
        public int SetLost;

        public Row(long teamId, string teamName, string? logoUrl)
        {
            TeamId = teamId;
            TeamName = teamName;
            LogoUrl = logoUrl;
        }

        public StandingEntry ToEntry(long leagueId, Sport sport)
        {
            var volleyball = sport == Sport.Volleyball;

            return new StandingEntry(
                leagueId,
                (int)TeamId,
                TeamName,
                LogoUrl,
                Played, Won, Drawn, Lost, Points, Scored, Conceded,
                volleyball ? SetWon : null,
                volleyball ? SetLost : null);
        }
    }
}
