namespace HotelsListing.Api.DomainObj
{
    public class Country
    {
        //here's ef use this as primary key 
        public int CountryId { get; set; }
        public string Name { get; set; }
        // use this as forign key
        public int HotelId { get; set; }
    public IList<Hotel>? Hotels { get; set; }= [];// that's to avoid null exptions 
        /*
          those tow lines ef use it to make the asssosiation between tabels 
          */
        //and use this as navigation property (one to many )
    }
}
