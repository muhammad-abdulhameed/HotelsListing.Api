using HotelsListing.Api.Dtos.HotelDtos;

namespace HotelsListing.Api.Dtos.CountryDtos
{
    public class UpdateCountryDto:PostCountryDto
    {
        public int CountryId { get; set; }
        public UpdateHotelDto? Hotel { get; set; }

    }
}
