using AutoMapper;
using AutoMapper.Execution;
using HotelsListing.Api.DomainObj;
using HotelsListing.Api.Dtos.CountryDtos;
using HotelsListing.Api.Dtos.HotelDtos;
using Microsoft.CodeAnalysis.FlowAnalysis;

namespace HotelsListing.Api.MappingProfiles
{
    public class HotelsMappingProfile:Profile
    {// here we create class that's seperate each obj we need (as profiles) to mapped to each other together the pass it to auto mapper 
        public HotelsMappingProfile()
        {
            CreateMap<Hotel, GetHotelDto>().ForMember(d=>d.CountryName,cfj=>cfj.MapFrom<CountryNameResolver>());
            CreateMap<UpdateHotelDto, Hotel>();
            CreateMap<PostHotelDto, Hotel>();
        }
    }

    public class CountryMappingProfile : Profile
    {// here we create class that's seperate each obj we need (as profiles) to mapped to each other together the pass it to auto mapper 
        public CountryMappingProfile()
        {
            CreateMap<Country, GetCountryDto>();
            CreateMap<GetCountryDto, Country >();
            CreateMap<UpdateCountryDto, Country>();
            CreateMap<PostCountryDto, Country>();
        }
    }
    public class CountryNameResolver : IValueResolver<Hotel, GetHotelDto, string>
    {
        public string Resolve(Hotel source, GetHotelDto destination, string destMember, ResolutionContext context)
        {
          return  source.Country?.Name ?? string.Empty;
        }
    }
     
    
}
