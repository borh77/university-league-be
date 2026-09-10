using Microsoft.EntityFrameworkCore;
using Solution.BuildingBlocks.Core.UseCases;
using Solution.BuildingBlocks.Infrastructure.Database;
using Solution.UniLeague.Core.Domain;
using Solution.UniLeague.Core.Domain.RepositoryInterfaces;
using System.Data;

namespace Solution.UniLeague.Infrastructure.Database.Repositories;

public class MatchDbRepository : IMatchRepository
{
    private readonly UniLeagueContext _dbContext;

    public MatchDbRepository(UniLeagueContext dbContext)
    {
        _dbContext = dbContext;
    }

    public PagedResult<Match> GetScheduleByLeague(long leagueId)
    {
        var task = _dbContext.Matches
            .Include("Result.Quarters")
            .Include("Result.Sets")
            .Include("Result.Goals")
            .Where(m => m.LeagueId == leagueId)
            .OrderBy(m => m.RoundNumber)
            .ThenBy(m => m.Stage)
            .ThenBy(m => m.ScheduledAt)
            .GetPaged(0, 0);
        task.Wait();
        return task.Result;
    }

    public PagedResult<Match> GetResultsByLeague(long leagueId)
    {
        var task = _dbContext.Matches
            .Include("Result.Quarters")
            .Include("Result.Sets")
            .Include("Result.Goals")
            .Where(m => m.LeagueId == leagueId && m.Result != null)
            .OrderBy(m => m.RoundNumber)
            .ThenBy(m => m.Stage)
            .ThenBy(m => m.ScheduledAt)
            .GetPaged(0, 0);
        task.Wait();
        return task.Result;
    }

    public Match? GetByIdWithResult(long matchId)
    {
        return _dbContext.Matches
            .Include("Result.Quarters")
            .Include("Result.Sets")
            .Include("Result.Goals")
            .Include("Result.PlayerStats")
            .FirstOrDefault(m => m.Id == matchId);
    }

    public void SaveResult(Match match)
    {
        // match je vec praćen iz GetByIdWithResult; zamena owned Result-a se detektuje sama
        if (_dbContext.Entry(match).State == EntityState.Detached)
            _dbContext.Matches.Update(match);

        _dbContext.SaveChanges();
    }

    public List<Match> GetAllPlayedByLeague(long leagueId)
    {
        return _dbContext.Matches
            .Include("Result.Sets")
            .Include("Result.Quarters")
            .Where(m => m.LeagueId == leagueId
                        && m.Stage == MatchStage.RegularSeason
                        && m.Result != null)
            .ToList();
    }

    public List<Match> GetRegularSeasonMatchesByLeague(long leagueId)
    {
        return _dbContext.Matches
            .Include("Result.Sets")
            .Include("Result.Quarters")
            .Where(m => m.LeagueId == leagueId && m.Stage == MatchStage.RegularSeason)
            .ToList();
    }

    public List<Match> GetPlayoffMatchesByLeague(long leagueId)
    {
        return _dbContext.Matches
            .Include("Result.Sets")
            .Include("Result.Quarters")
            .Where(m => m.LeagueId == leagueId && m.Stage != MatchStage.RegularSeason)
            .ToList();
    }

    public bool HasPlayoffSemifinals(long leagueId)
    {
        return _dbContext.Matches
            .Any(m => m.LeagueId == leagueId && m.Stage == MatchStage.PlayoffSemifinal);
    }

    public bool AddPlayoffSemifinalsIfNone(long leagueId, IReadOnlyCollection<Match> matches)
    {
        return AddPlayoffMatchesIfStagesMissing(
            leagueId,
            new[] { MatchStage.PlayoffSemifinal },
            matches);
    }

    public bool AddPlayoffMatchesIfStagesMissing(
        long leagueId,
        IReadOnlyCollection<MatchStage> stages,
        IReadOnlyCollection<Match> matches)
    {
        if (stages.Count == 0 || matches.Count == 0)
            return false;

        using var transaction = _dbContext.Database.BeginTransaction(IsolationLevel.Serializable);

        var existingStages = _dbContext.Matches
            .Where(m => m.LeagueId == leagueId && stages.Contains(m.Stage))
            .Select(m => m.Stage)
            .ToHashSet();

        var missingStageMatches = matches
            .Where(m => !existingStages.Contains(m.Stage))
            .ToList();

        if (missingStageMatches.Count == 0)
        {
            transaction.Commit();
            return false;
        }

        _dbContext.Matches.AddRange(missingStageMatches);
        _dbContext.SaveChanges();
        transaction.Commit();

        return true;
    }
}
