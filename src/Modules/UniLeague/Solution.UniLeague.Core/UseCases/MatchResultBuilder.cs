using Solution.UniLeague.API.Dtos;
using Solution.UniLeague.Core.Domain;
using Solution.UniLeague.Core.Domain.RepositoryInterfaces;

namespace Solution.UniLeague.Core.UseCases;

public class MatchResultBuilder : IMatchResultBuilder
{
    private readonly ITeamRepository _teamRepository;

    public MatchResultBuilder(ITeamRepository teamRepository)
    {
        _teamRepository = teamRepository;
    }

    public MatchResult Build(Sport sport, Match match, SubmitMatchResultDto request)
    {
        return sport switch
        {
            Sport.Basketball => request.PlayerStats is { Count: > 0 }
                ? MatchResult.CreateWithPlayerStats(
                    request.HomeScore, request.AwayScore,
                    ResolvePlayerStats(match, request.PlayerStats),
                    quarters: RequireQuarters(request))
                : MatchResult.CreateWithQuarters(
                    request.HomeScore, request.AwayScore, RequireQuarters(request)),

            Sport.Volleyball => request.PlayerStats is { Count: > 0 }
                ? MatchResult.CreateWithPlayerStats(
                    request.HomeScore, request.AwayScore,
                    ResolvePlayerStats(match, request.PlayerStats),
                    sets: RequireSets(request))
                : MatchResult.CreateWithSets(
                    request.HomeScore, request.AwayScore, RequireSets(request)),

            _ => request.Goals is { Count: > 0 }
                ? MatchResult.CreateWithGoals(
                    request.HomeScore, request.AwayScore,
                    request.Goals.Select(g => GoalEvent.Create(
                        g.ScorerName, g.TeamName, g.IsHomeTeamGoal, g.Minute)))
                : MatchResult.Create(request.HomeScore, request.AwayScore)
        };
    }

    private static List<QuarterScore> RequireQuarters(SubmitMatchResultDto request)
    {
        if (request.Quarters is not { Count: > 0 })
            throw new ArgumentException("Quarters are required for basketball results.");
        return request.Quarters
            .Select(q => QuarterScore.Create(q.QuarterNumber, q.HomeScore, q.AwayScore))
            .ToList();
    }

    private static List<SetScore> RequireSets(SubmitMatchResultDto request)
    {
        if (request.Sets is not { Count: > 0 })
            throw new ArgumentException("Sets are required for volleyball results.");
        return request.Sets
            .Select(s => SetScore.Create(s.SetNumber, s.HomeScore, s.AwayScore))
            .ToList();
    }

    private List<PlayerStatLine> ResolvePlayerStats(Match match, List<PlayerStatInputDto> inputs)
    {
        var homePlayers = _teamRepository.GetByIdWithPlayers(match.HomeTeamId).Players;
        var awayPlayers = _teamRepository.GetByIdWithPlayers(match.AwayTeamId).Players;

        return inputs.Select(input =>
        {
            var roster = input.IsHomeTeam ? homePlayers : awayPlayers;
            var player = roster.FirstOrDefault(p => p.Id == input.PlayerId)
                ?? throw new ArgumentException(
                    $"Player {input.PlayerId} is not in the {(input.IsHomeTeam ? "home" : "away")} team roster.");

            return PlayerStatLine.Create(
                player.Id,
                $"{player.FirstName} {player.LastName}",
                player.JerseyNumber,
                input.IsHomeTeam,
                input.Points);
        }).ToList();
    }
}
