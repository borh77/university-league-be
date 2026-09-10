using ArchUnitNET.xUnit;
using static ArchUnitNET.Fluent.ArchRuleDefinition;

namespace Solution.Architecture.Tests;

public class ModulesTests : BaseArchitecturalTests
{
    [Theory]
    [MemberData(nameof(GetModules))]
    public void API_projects_should_only_reference_themselves_and_core_building_blocks(string moduleName)
    {
        var examinedTypes = GetExaminedTypes($"Solution.{moduleName}.API");
        var forbiddenTypes = GetForbiddenTypes("Solution.BuildingBlocks.Core", $"Solution.{moduleName}.API");

        var rule = Types().That().Are(examinedTypes).Should().NotDependOnAny(forbiddenTypes).WithoutRequiringPositiveResults();

        rule.Check(Architecture);
    }

    [Theory]
    [MemberData(nameof(GetModules))]
    public void Core_projects_should_only_reference_themselves_API_projects_and_core_building_blocks(string moduleName)
    {
        var examinedTypes = GetExaminedTypes($"Solution.{moduleName}.Core");
        var forbiddenTypes = GetForbiddenTypes("Solution.BuildingBlocks.Core", "Solution\\..+\\.API", $"Solution.{moduleName}.Core");

        var rule = Types().That().Are(examinedTypes).Should().NotDependOnAny(forbiddenTypes);

        rule.Check(Architecture);
    }

    [Theory]
    [MemberData(nameof(GetModules))]
    public void Infra_projects_should_only_reference_themselves_their_API_and_core_projects_and_building_blocks(string moduleName)
    {
        var examinedTypes = GetExaminedTypes($"Solution.{moduleName}.Infrastructure");
        var forbiddenTypes = GetForbiddenTypes("Solution.BuildingBlocks.", $"Solution.{moduleName}.");

        var rule = Types().That().Are(examinedTypes).Should().NotDependOnAny(forbiddenTypes);

        rule.Check(Architecture);
    }

    [Theory]
    [MemberData(nameof(GetModules))]
    public void Domain_namespaces_should_only_reference_themselves_and_core_building_blocks(string moduleName)
    {
        var allTypesFromCoreAssembly = GetExaminedTypes($"Solution.{moduleName}.Core").ToList();
        var domainTypes = allTypesFromCoreAssembly.Where(x => x.FullName.Contains(".Domain.")).ToList();
        var nonDomainTypes = allTypesFromCoreAssembly.Where(x => !x.FullName.Contains(".Domain."));
        var typesFromOtherAssemblies = GetForbiddenTypes("Solution.BuildingBlocks.Core", $"Solution.{moduleName}.Core");

        var otherAssemblyRule = Types().That().Are(domainTypes).Should().NotDependOnAny(typesFromOtherAssemblies).WithoutRequiringPositiveResults();
        var sameAssemblyRule = Types().That().Are(domainTypes).Should().NotDependOnAny(nonDomainTypes).WithoutRequiringPositiveResults();

        otherAssemblyRule.Check(Architecture);
        sameAssemblyRule.Check(Architecture);
    }

    [Theory]
    [MemberData(nameof(GetModules))]
    public void Services_should_not_reference_public_APIs_of_other_modules(string moduleName)
    {
        var allTypesFromCoreAssembly = GetExaminedTypes($"Solution.{moduleName}.Core").ToList();
        var useCaseTypes = allTypesFromCoreAssembly.Where(x => x.FullName.Contains(".UseCases.")).ToList();
        var typesFromOtherAssemblies = GetForbiddenTypes("Solution.API", $"Solution.{moduleName}.API");
        var publicApiTypesFromOtherAssemblies = typesFromOtherAssemblies.Where(x => x.FullName.Contains("API.Public"));

        var rule = Types().That().Are(useCaseTypes).Should().NotDependOnAny(publicApiTypesFromOtherAssemblies).WithoutRequiringPositiveResults();

        rule.Check(Architecture);
    }

    [Fact]
    public void Web_API_should_not_reference_internal_APIs_of_modules()
    {
        var apiTypes = GetExaminedTypes("Solution.API").ToList();
        var typesFromOtherAssemblies = GetForbiddenTypes("Solution.API");
        var internalApiTypes = typesFromOtherAssemblies.Where(x => x.FullName.Contains("API.Internal"));

        var rule = Types().That().Are(apiTypes).Should().NotDependOnAny(internalApiTypes);

        rule.Check(Architecture);
    }

    /// <summary>
    /// The single business module in the solution.
    /// Add more entries here as new modules are introduced.
    /// </summary>
    public static IEnumerable<object[]> GetModules() => new List<object[]>
    {
        new object[] { "UniLeague" },
        new object[] { "Identity" }
    };
}
