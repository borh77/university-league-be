using AutoMapper.Configuration.Conventions;
using Solution.BuildingBlocks.Core.UseCases;
using Solution.UniLeague.Core.Domain;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Solution.UniLeague.Core.Domain.RepositoryInterfaces;

public interface IMatchRepository
{
    public PagedResult<Match> GetScheduleByLeague(long leagueId);
    public PagedResult<Match> GetResultsByLeague(long leagueId);

    public Match? GetByIdWithResult(long matchId);
    public void SaveResult(Match match);

    public List<Match> GetAllPlayedByLeague(long leagueId);
    public List<Match> GetRegularSeasonMatchesByLeague(long leagueId);
    public List<Match> GetPlayoffMatchesByLeague(long leagueId);
    public bool HasPlayoffSemifinals(long leagueId);
    public bool AddPlayoffSemifinalsIfNone(long leagueId, IReadOnlyCollection<Match> matches);
    public bool AddPlayoffMatchesIfStagesMissing(
        long leagueId,
        IReadOnlyCollection<MatchStage> stages,
        IReadOnlyCollection<Match> matches);
}
