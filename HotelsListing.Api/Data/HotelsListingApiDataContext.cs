using HotelsListing.Api.DomainObj;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using System.Reflection;

namespace HotelsListing.Api.Data
{                                               // that's interface that's contain identity tapels class also 
    public class HotelsListingApiDataContext:IdentityDbContext<AppUser>
    {
        //here before you setup the data base you should init the configration (like db type name port ) so you have to pass this to parent class and inject it on program start up 
        public HotelsListingApiDataContext(DbContextOptions<HotelsListingApiDataContext> options ):base(options) { }
        // DbSet is class on ef that's make yor domain obj as consptual obj on  data base 
        public DbSet<Hotel>Hotels { get; set; }
        public DbSet<Country> Countries { get; set; }
        public DbSet<HotelAdmin> HotelAdmins { get; set; }
        public DbSet<Booking> Bookings { get; set; }

        protected override void OnModelCreating(ModelBuilder builder)
        { // this method called on model creating 
            base.OnModelCreating(builder);

            // this fetech for configration fils that's related to db then configere it 
            builder.ApplyConfigurationsFromAssembly(Assembly.GetExecutingAssembly());
        }
    
    }
}
