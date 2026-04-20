using HotelsListing.Api.Data;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.EntityFrameworkCore;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;

namespace HotelsListing.Api.AuthrizationFilters;

/*
 1️ Request comes
2️ Authorization filter runs
3️ If allowed → controller executes
4️ If not allowed → return 401 or 403

Request → Middleware → Authorization Filter (your filter) → Controller
 */


// this to make abilty to treat this class as attrbuite
[AttributeUsage(AttributeTargets.Class | AttributeTargets.Method, AllowMultiple = false)]
                                                //this class make ability to inject the class that's implement the auth filter on you wraper class
public sealed class HotelOrSystemAdminAttribute : TypeFilterAttribute
{// that's wraper class 
    public HotelOrSystemAdminAttribute() : base(typeof(HotelOrSystemAdminFilter))
    {
    }
}
public class HotelOrSystemAdminFilter(HotelsListingApiDataContext dbContext) : IAsyncAuthorizationFilter
{
    public async Task OnAuthorizationAsync(AuthorizationFilterContext context)
    {
        var httpUser = context.HttpContext.User;

        if (httpUser?.Identity?.IsAuthenticated == false)
        {
            context.Result = new UnauthorizedResult();
            return;
        }

        // If user is a global Administrator, allow immediately
        if (httpUser!.IsInRole("Administrator"))
        {
            return;
        }

        var userId = httpUser.FindFirst(JwtRegisteredClaimNames.Sub)?.Value ?? httpUser.FindFirst(ClaimTypes.NameIdentifier)?.Value;

        if (string.IsNullOrWhiteSpace(userId))
        {
            context.Result = new ForbidResult();
            return;
        }

        // Try to get hotelId from route values
        context.RouteData.Values.TryGetValue("hotelId", out var hotelIdObj);
        int.TryParse(hotelIdObj?.ToString(), out int hotelId);
        if (hotelId == 0)
        {
            context.Result = new ForbidResult();
            return;
        }

        // Check if user is an admin for this specific hotel
        var isHotelAdminUser = await dbContext.HotelAdmins
            .AnyAsync(q => q.UserID == userId && q.HotelId == hotelId);

        if (!isHotelAdminUser)
        {
            context.Result = new ForbidResult();
            return;
        }
    }
}
