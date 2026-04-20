namespace HotelsListing.Api.DomainObj;

public class HotelAdmin
{
   public  int Id { get; set; }
    public required string Name { get; set; }

    public required int HotelId { get; set; }
    public Hotel? Hotel { get; set; }
    public AppUser? User { get; set; }
    public string? UserID { get; set; }
    // that's string cause identity class treates identity as strings

}
// note here we make compostion not make new enetiy to seperate auth service from business logic 