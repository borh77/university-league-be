using Solution.API.Controllers.Administrator.Administration;
using Solution.BuildingBlocks.Core.UseCases;
using Solution.Tours.API.Dtos;
using Solution.Tours.API.Public.Administration;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.DependencyInjection;
using Shouldly;

namespace Solution.Tours.Tests.Integration.Administration;

[Collection("Sequential")]
public class EquipmentQueryTests : BaseToursIntegrationTest
{
    public EquipmentQueryTests(ToursTestFactory factory) : base(factory) { }

    [Fact]
    public void Retrieves_all()
    {
        // Arrange
        using var scope = Factory.Services.CreateScope();
        var controller = CreateController(scope);

        // Act
        var result = ((ObjectResult)controller.GetAll(0, 0).Result)?.Value as PagedResult<EquipmentDto>;

        // Assert
        result.ShouldNotBeNull();
        result.Results.Count.ShouldBe(3);
        result.TotalCount.ShouldBe(3);
    }

    private static EquipmentController CreateController(IServiceScope scope)
    {
        return new EquipmentController(scope.ServiceProvider.GetRequiredService<IEquipmentService>())
        {
            ControllerContext = BuildContext("-1")
        };
    }
}