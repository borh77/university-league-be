using Solution.UniLeague.API.Dtos;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Solution.UniLeague.Core.UseCases
{
    public interface ITopScorerQueryService
    {
        List<TopScorerDto> GetTopScorersByLeague(long leagueId);
    }
}
