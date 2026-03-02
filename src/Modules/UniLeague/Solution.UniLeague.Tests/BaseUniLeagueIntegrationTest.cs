using Solution.BuildingBlocks.Tests;
using Solution.UniLeague.Infrastructure.Database;

namespace Solution.UniLeague.Tests;

public abstract class BaseUniLeagueIntegrationTest : BaseWebIntegrationTest<UniLeagueTestFactory>
{
    protected BaseUniLeagueIntegrationTest(UniLeagueTestFactory factory) : base(factory) { }
}
