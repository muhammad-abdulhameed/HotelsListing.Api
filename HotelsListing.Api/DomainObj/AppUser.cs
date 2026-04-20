using Microsoft.AspNetCore.Identity;
using System.ComponentModel.DataAnnotations.Schema;

namespace HotelsListing.Api.DomainObj
{
    public class AppUser:IdentityUser
    {
        public string FirstName {set; get; } = string.Empty;
        public string LastName {set; get;  } = string.Empty;

        [NotMapped]
        public  string  FullName  => FirstName+""+LastName;

    }
}
