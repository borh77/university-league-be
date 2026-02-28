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
}