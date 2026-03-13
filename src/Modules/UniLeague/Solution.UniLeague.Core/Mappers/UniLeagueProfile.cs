using AutoMapper;
using Solution.UniLeague.API.Dtos;
using Solution.UniLeague.Core.Domain;

namespace Solution.UniLeague.Core.Mappers;

public class UniLeagueProfile : Profile
{
    public UniLeagueProfile()
    {
        AllowNullCollections = true;

        CreateMap<StandingEntry, StandingsRowDto>()
            .ForMember(dest => dest.Difference, opt => opt.MapFrom(src => src.Difference))
            .ForMember(dest => dest.SetDifference, opt => opt.MapFrom(src => src.SetDifference))
            .ForMember(dest => dest.Position, opt => opt.Ignore()); // Position se dodeljuje nakon sortiranja

        CreateMap<QuarterScore, QuarterScoreDto>();

        CreateMap<Match, MatchDto>()
            .ForMember(dest => dest.Result,
                opt => opt.MapFrom(src => src.Result != null ? src.Result.ToString() : null))
            .ForMember(dest => dest.Quarters,
                opt => opt.MapFrom(src => src.Result != null && src.Result.HasQuarters ? src.Result.Quarters : null));

        CreateMap<Player, PlayerDto>().ReverseMap();
        CreateMap<Team, TeamProfileDto>().ReverseMap();
    }
}