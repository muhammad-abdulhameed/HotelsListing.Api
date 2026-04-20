namespace HotelsListing.Api.Dtos.HotelDtos;

public class GetHotelDto { 
   public int Id { set; get; } 
   public string Name { set; get; }
   public double Rating { set; get; }
   public int CountryId { set; get; }
    public string CountryName { set; get; }
};
