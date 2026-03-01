using AutoMapper;
using Solution.UniLeague.API.Dtos;
using Solution.UniLeague.Core.Domain;

namespace Solution.UniLeague.Core.Mappers;

/// <summary>
/// AutoMapper profile for UniLeague module – add mappings in Phase 1.
/// </summary>
public class UniLeagueProfile : Profile
{
    public UniLeagueProfile()
    {
        CreateMap<Match, MatchDto>()
            .ForMember(dest => dest.Result,
                opt => opt.MapFrom(src => src.Result != null ? src.Result.ToString() : null))
            .ReverseMap();

        CreateMap<Player, PlayerDto>().ReverseMap();
        CreateMap<Team, TeamProfileDto>().ReverseMap();
    }
}
