using Solution.BuildingBlocks.Tests;

namespace Solution.Tours.Tests;

public class BaseToursIntegrationTest : BaseWebIntegrationTest<ToursTestFactory>
{
    public BaseToursIntegrationTest(ToursTestFactory factory) : base(factory) { }
}