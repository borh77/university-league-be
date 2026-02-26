using AutoMapper;
using Solution.Tours.API.Dtos;
using Solution.Tours.Core.Domain;

namespace Solution.Tours.Core.Mappers;

public class ToursProfile : Profile
{
    public ToursProfile()
    {
        CreateMap<EquipmentDto, Equipment>().ReverseMap();
    }
}