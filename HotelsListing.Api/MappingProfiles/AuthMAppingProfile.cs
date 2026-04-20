using AutoMapper;
using HotelsListing.Api.DomainObj;
using HotelsListing.Api.Dtos.Auth;
using HotelsListing.Api.Dtos.HotelDtos;

namespace HotelsListing.Api.MappingProfiles;

public class AuthMAppingProfile:Profile
{
    public AuthMAppingProfile()
    {
        CreateMap<RegisterUserDto, AppUser>();
        CreateMap<AppUser, RegisterUserDto>();
    }
}

public class UserNameResolver : IValueResolver<AppUser, RegisterUserDto, string>
{
    public string Resolve(AppUser source, RegisterUserDto destination, string destMember, ResolutionContext context)
    {
        return source?.UserName ?? string.Empty;
    }

    
}


