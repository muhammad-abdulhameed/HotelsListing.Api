
namespace HotelListing.Api.Constants;
//that's constant values for aauth
public class AuthenticationDefaults
{   // that's constant value for basic auth on Authrization header 
    public const string BasicScheme = "Basic";
    // that's constant value for Api key auth on Authrization header 
    public const string ApiKeyScheme = "ApiKey";
    // that's constant and standerd name of header field that's has api key   on Authrization header 
    public const string ApiKeyHeaderName = "X-Api-Key";
    public const string AppName = "HotelListingApi";
    public const string SystemAdminRole = "Admin";
    public const string HotelAdminRole = "HotelAdmin";
}