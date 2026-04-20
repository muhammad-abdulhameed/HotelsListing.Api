using System.ComponentModel.DataAnnotations;

namespace HotelsListing.Api.Dtos.HotelDtos
{
    public class PostHotelDto
    {
        [MaxLength(255)]
        public required string Name { get; set; }
        [Range(1,5)]
        public required double Rating { get; set; }
        public required int CountryId { get; set; }
    }
}
