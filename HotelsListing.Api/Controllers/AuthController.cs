using AutoMapper;
using HotelListing.Api.Results;
using HotelsListing.Api.Contructs;
using HotelsListing.Api.Dtos.Auth;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;

namespace HotelsListing.Api.Controllers;

[Route("Authapi/[controller]")]
[ApiController]
[AllowAnonymous]
public class AuthController(IAuthService authService) : BaseApiController
{
    [HttpPost("register")]
    public async Task<ActionResult<RegisterUserDto>> Register(RegisterUserDto userDto)
    {
        
        var result = await authService.Register(userDto);

        return ToActionResult(result);

    }
    [HttpPost("login")]
    public async Task<ActionResult<string>> Login(LoginUserDto userDto)
    {

        var result = await authService.Login(userDto);

        return ToActionResult(result);

    }
}
