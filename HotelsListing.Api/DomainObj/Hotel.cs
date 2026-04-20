using System.Collections;

namespace HotelsListing.Api.DomainObj
{
    public class Hotel
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public double Rating { get; set; }
        public decimal PerNightRate { get; set; }
        public int CountryId { get; set; }
        public Country? Country { get; set; }

        public ICollection<HotelAdmin>? HotelAdmins { get; set; } = [];
        public ICollection<Booking>? Bookings { get; set; } = [];
    }
}
