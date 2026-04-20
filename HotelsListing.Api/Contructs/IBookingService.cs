using HotelListing.Api.Results;
using HotelsListing.Api.Dtos.Bookings;
using HotelsListing.common.Models.Paging;

namespace HotelsListing.Api.Contructs
{
    public interface IBookingService
    {
        Task<Result<GetBookingDto>> CreateBookingAsync(CreateBookingDto createBookingDto);
        Task<Result<GetBookingDto>> UpdateBookingAsync(int hotelId,int bookingId,UpdateBookingDto createBookingDto);
        Task<Result> CancelBookingAsync(int hotelId,int bookingId);
        Task<Result<PagedResult<GetBookingDto>>> GetBookingsAsync(int hotelId, PaginationParameters paginationParameters);

        
    }
}