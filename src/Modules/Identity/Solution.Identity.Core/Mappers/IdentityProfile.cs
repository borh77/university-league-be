using AutoMapper;
using Solution.Identity.API.Dtos;
using Solution.Identity.Core.Domain;

namespace Solution.Identity.Core.Mappers;

public class IdentityProfile : Profile
{
    public IdentityProfile()
    {
        CreateMap<User, AuthResponseDto>()
            .ForMember(dest => dest.Role, opt => opt.MapFrom(src => src.Role.ToString()))
            .ForMember(dest => dest.Token, opt => opt.Ignore())
            .ForMember(dest => dest.ExpiresAt, opt => opt.Ignore());
    }
}
