using ArchUnitNET.xUnit;
using static ArchUnitNET.Fluent.ArchRuleDefinition;

namespace Solution.Architecture.Tests;

public class BuildingBlocksTests : BaseArchitecturalTests
{
    [Fact]
    public void Core_should_not_reference_other_projects()
    {
        var examinedTypes = GetExaminedTypes("Solution.BuildingBlocks.Core");
        var forbiddenTypes = GetForbiddenTypes("Solution.BuildingBlocks.Core");

        var rule = Types().That().Are(examinedTypes).Should().NotDependOnAny(forbiddenTypes);

        rule.Check(Architecture);
    }

    [Fact]
    public void Infrastructure_should_not_reference_other_projects_apart_from_core()
    {
        var examinedTypes = GetExaminedTypes("Solution.BuildingBlocks.Infrastructure");
        var forbiddenTypes = GetForbiddenTypes("Solution.BuildingBlocks.Infrastructure", "Solution.BuildingBlocks.Core");

        var rule = Types().That().Are(examinedTypes).Should().NotDependOnAny(forbiddenTypes);

        rule.Check(Architecture);
    }
}