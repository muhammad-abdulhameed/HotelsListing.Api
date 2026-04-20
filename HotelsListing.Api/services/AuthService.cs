using AutoMapper;
using HotelListing.Api.Results;
using HotelsListing.Api.Contructs;
using HotelsListing.Api.DomainObj;
using HotelsListing.Api.Dtos.Auth;
using HotelsListing.common.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.CodeAnalysis.Options;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using Microsoft.VisualStudio.Web.CodeGenerators.Mvc.Templates.BlazorIdentity.Pages.Manage;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace HotelsListing.Api.services;

public class AuthService(IMapper mapper, UserManager<AppUser> userManager,IOptions<JwtSettings> options /*IConfiguration configuration*/, IHttpContextAccessor httpContextAccessor) : IAuthService
{
    public async Task<Result<RegisterUserDto>> Register(RegisterUserDto userDto)
    {
        var isEmailExist = await userManager.FindByEmailAsync(userDto.Email);

        if (isEmailExist != null)
        {
            return Result<RegisterUserDto>.BadRequest(new Error(ErrorCodes.BadRequest, "this Acccount already exist"));
        }
        { }
        AppUser user = new AppUser { Email = userDto.Email, UserName = userDto.Email,FirstName=userDto.FirstName,LastName=userDto.LastName};
        user.UserName = user.Email;
        try
        {
            var result = await userManager.CreateAsync(user,userDto.Password);

            if (!result.Succeeded)
            {
                var errors = result.Errors.Select(e => new Error(ErrorCodes.BadRequest, e.Description)).ToArray();
                return Result<RegisterUserDto>.BadRequest(errors);
            }

           await userManager.AddToRoleAsync(user, userDto.Role);

            

            return Result<RegisterUserDto>.Success(userDto);
        }
        catch (Exception e)
        {
            return Result<RegisterUserDto>.Failure(new Error(ErrorCodes.Failure, $"{e.Message} the exption "));
        }


    }


    public async Task<Result<string>> Login(LoginUserDto userDto)
    {


        try
        {
            var result = await userManager.FindByEmailAsync(userDto.Email);

            if (result is null)
            {
                return Result<string>.Failure(new Error(ErrorCodes.BadRequest, "Invalid credentials."));
            }

            var token =await GenerateToken(result);
           var role= await userManager.GetRolesAsync(result);


            return Result<string>.Success($"{token}\n {role[0]} ");
        }
        catch
        {
            return Result<string>.Failure(new Error(ErrorCodes.BadRequest, "Invalid credentials."));
        }


    }

    public string UserId => httpContextAccessor?
             .HttpContext?
             .User?
             .FindFirst(JwtRegisteredClaimNames.Sub)?.Value
         ?? httpContextAccessor?
             .HttpContext?/*
                           * here we add if not find the id on sup 
                           * nameIdentfier is also contain user id
                           */
             .User?
             .FindFirst(ClaimTypes.NameIdentifier)?.Value
         ?? string.Empty;
    private async Task<string> GenerateToken(AppUser user)
    {
        // set our user claims
        var claims = new List<Claim>
        {
            new Claim(JwtRegisteredClaimNames.Sub,user.Id),
            new Claim(JwtRegisteredClaimNames.Name,user.FullName),
            new Claim(JwtRegisteredClaimNames.Email,user.Email),
            new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString()),

            
        };
        // set user roles
        var roles = await userManager.GetRolesAsync(user);
        var roleClaims = roles.Select(x => new Claim(ClaimTypes.Role, x));

        claims.Union(roleClaims).ToList();

        //set securty key crdaintial 

        var securityKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(options.Value.Key /*configuration["JwtSettings:SecretKey"]*/));
        /*
         * we replace all direct access configration json by binded option class (option pattern)
         */
        var credentials = new SigningCredentials(securityKey, SecurityAlgorithms.HmacSha256);

        // Create an encoded token
        var token = new JwtSecurityToken(
            issuer: options.Value.Issuer/*configuration["JwtSettings:Issuer"]*/,
            audience: options.Value.Audience/* configuration["JwtSettings:Audience"]*/,
            claims: claims,
            expires: DateTime.UtcNow.AddMinutes(Convert.ToInt32(options.Value.DurationInMinutes /*configuration["JwtSettings:DurationInMinutes"]*/)),
            signingCredentials: credentials
            );

        // Return token value
        return new JwtSecurityTokenHandler().WriteToken(token);
    } 
}
