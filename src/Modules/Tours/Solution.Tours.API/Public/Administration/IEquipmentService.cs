using Solution.BuildingBlocks.Core.UseCases;
using Solution.Tours.API.Dtos;

namespace Solution.Tours.API.Public.Administration;

public interface IEquipmentService
{
    PagedResult<EquipmentDto> GetPaged(int page, int pageSize);
    EquipmentDto Create(EquipmentDto equipment);
    EquipmentDto Update(EquipmentDto equipment);
    void Delete(long id);
}