using AutoMapper;
using Solution.UniLeague.API.Dtos;
using Solution.UniLeague.Core.Domain;

namespace Solution.UniLeague.Core.Mappers;

public class UniLeagueProfile : Profile
{
    public UniLeagueProfile()
    {
        CreateMap<StandingEntry, StandingsRowDto>()
            .ForMember(dest => dest.Difference, opt => opt.MapFrom(src => src.Difference))
            .ForMember(dest => dest.SetDifference, opt => opt.MapFrom(src => src.SetDifference))
            .ForMember(dest => dest.Position, opt => opt.Ignore()); // Position se dodeljuje nakon sortiranja
    }
}