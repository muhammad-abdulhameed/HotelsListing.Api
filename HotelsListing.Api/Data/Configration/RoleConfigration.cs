using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace HotelsListing.Api.Data.Configration
{// this class to apply some default configration on spcific table (in our case we add default data for roles table)
    public class RoleConfigration : IEntityTypeConfiguration<IdentityRole>
    {//builder.ApplyConfigurationsFromAssembly(Assembly.GetExecutingAssembly()); this class will apply by this method that's on Db class
        // and ecxute this method to apply our configration on next migration
        public void Configure(EntityTypeBuilder<IdentityRole> builder)
        {
            // that's the default roles we need to apply once system are created
            builder.HasData(
                new IdentityRole 
                {// here we add predefind id to unique the role before it migrated (dont make db generate it )
                    Id= "323c3234-6390-4565-9ee1-670aca04a024",
                    Name = "Admin",
                    NormalizedName="ADMIN"
                },

                new IdentityRole
                {
                    Id = "1b14431d-bf86-40d5-821f-3ce9ad01765a",
                    Name = "User",
                    NormalizedName = "USER"
                },
                new IdentityRole
                {
                    Id = "1b14431d-bf86-40d5-821f-670aca04a024",
                    Name = "HotelAdmin",
                    NormalizedName = "HOTELADMIN"
                }
                );
        }
    }
}
