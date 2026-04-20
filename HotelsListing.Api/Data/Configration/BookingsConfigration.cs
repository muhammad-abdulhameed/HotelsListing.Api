using HotelsListing.Api.DomainObj;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace HotelsListing.Api.Data.Configration
{
    public class BookingsConfigration : IEntityTypeConfiguration<Booking>
    {
        public void Configure(EntityTypeBuilder<Booking> builder)
        {// here we convert enum values to the string value cause ef by default uses the int value 
            builder.Property(q => q.Status).HasConversion<string>().HasMaxLength(20);
            // here we add index for this properties for fast look ups and only this proprties cause it not frequntly changable
            builder.HasIndex(q => q.UserId);
            builder.HasIndex(q => q.HotelId);
            // that's called composite index
            builder.HasIndex(q => new { q.CheckIn,q.CheckOut } );
        }
    }
}
