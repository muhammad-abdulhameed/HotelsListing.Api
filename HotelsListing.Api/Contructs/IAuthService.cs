using HotelListing.Api.Results;
using HotelsListing.Api.Dtos.Auth;

namespace HotelsListing.Api.Contructs
{
    public interface IAuthService
    {
        string UserId { get; }

        Task<Result<string>> Login(LoginUserDto userDto);
        Task<Result<RegisterUserDto>> Register(RegisterUserDto userDto);
    }
}