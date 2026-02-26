using Solution.BuildingBlocks.Tests;

namespace Solution.Stakeholders.Tests;

public class BaseStakeholdersIntegrationTest : BaseWebIntegrationTest<StakeholdersTestFactory>
{
    public BaseStakeholdersIntegrationTest(StakeholdersTestFactory factory): base(factory) {}
}