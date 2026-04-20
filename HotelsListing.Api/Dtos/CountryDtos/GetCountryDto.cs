using HotelsListing.Api.DomainObj;
using HotelsListing.Api.Dtos.HotelDtos;

namespace HotelsListing.Api.Dtos.CountryDtos
{
    public class GetCountryDto:GetHotelsDto
    {
        public IEnumerable<GetHotelsDto>? Hotels { get; set; } = [];
    }
}
