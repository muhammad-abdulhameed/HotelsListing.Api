using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System.ComponentModel.DataAnnotations;

namespace HotelsListing.Api.Dtos.Auth;

public class RegisterUserDto :LoginUserDto
{
    [MaxLength(50)]
    public required string FirstName { set; get; } = string.Empty;
    [MaxLength(50)]
    public required string LastName { set; get; } = string.Empty;
    [Phone]
    public required string  PhoneNumber { get; set; } = string.Empty;
    public  string  Role { get; set; } = "User";
  
}
