using Solution.BuildingBlocks.Tests;

namespace Solution.Blog.Tests;

public class BaseBlogIntegrationTest : BaseWebIntegrationTest<BlogTestFactory>
{
    public BaseBlogIntegrationTest(BlogTestFactory factory) : base(factory) { }
}