namespace HotelsListing.Api.Dtos.Bookings;

public class GetBookingDto
{
   public int Id { get; set; }
    public int HotelId { get; set; }
    public required string HotelName { get; set; }
    public DateOnly CheckIn { get; set; }
    public DateOnly CheckOut { get; set; }
    public int Guests { get; set; }
    public decimal TotalPrice { get; set; }
    public  string? Status { get; set; }
    public DateTime CreatedAtUtc { get; set; }
    public DateTime? UpdatedAtUtc { get; set; }
}
