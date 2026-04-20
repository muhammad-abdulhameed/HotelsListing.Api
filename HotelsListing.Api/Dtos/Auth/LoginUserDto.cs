using System.ComponentModel.DataAnnotations;

namespace HotelsListing.Api.Dtos.Auth;

public class LoginUserDto
{
    [EmailAddress]
    public required string Email { get; set; }
    [Required]
    public required string Password { get; set; }
    

}
