using HotelListing.Api.Results;
using HotelsListing.Api.Contructs;
using HotelsListing.Api.Data;
using HotelsListing.Api.DomainObj;
using HotelsListing.Api.DomainObj.Enums;
using HotelsListing.Api.Dtos.Bookings;
using HotelsListing.common.Extensions;
using HotelsListing.common.Models.Filtering;
using HotelsListing.common.Models.Paging;
using Microsoft.EntityFrameworkCore;
using System.IdentityModel.Tokens.Jwt;
using System.Threading.Tasks;

namespace HotelsListing.Api.services;
// this accessor to get data from current request 
public class BookingService(HotelsListingApiDataContext context, IAuthService authService ) : IBookingService
{
    public async Task<Result<PagedResult<GetBookingDto>>> GetBookingsAsync(int hotelId, PaginationParameters paginationParameters/*,BookingFilterParameters bookingFilterParameters*/ )
    {
        // here  we reblace the logic of chicking is admin is access to this hotel  replaced on the auth filter that we add the auth logic on 
        var isHotelExist = await context.Bookings.AnyAsync(q => q.HotelId == hotelId);
        if (!isHotelExist)
            return Result<PagedResult<GetBookingDto>>.NotFound(new Error(ErrorCodes.NotFound, $"This hotel by this id {hotelId} is not exist b "));

       /* var query = context.Hotels.AsQueryable();
        if(bookingFilterParameters.)*/

        var bookings = await context.Bookings
            .Where(h => h.HotelId == hotelId)
            .OrderBy(h => h.CheckIn)
            .Select(h => new GetBookingDto
            {
                Id = h.Id,
                HotelName = h.Hotel.Name,
                HotelId = h.Hotel.Id,
                CheckIn = h.CheckIn,
                CheckOut = h.CheckOut,
                CreatedAtUtc = h.CreatedAtUtc,
                Guests = h.Guests,
                Status = h.Status.ToString(),
                TotalPrice = h.TotalPrice,
                UpdatedAtUtc = h.UpdatedAtUtc,
                // that's the extintion method that we add to Iqurable for impl the paging on source (this query)
            }).ToPagedResultAsync(paginationParameters ); /*ToListAsync();*/
        return Result<PagedResult<GetBookingDto>>.Success(bookings);
    }

    public async Task<Result<GetBookingDto>> CreateBookingAsync(CreateBookingDto createBookingDto)
    {
        // get user id from claims 

        var userId = authService.UserId; 

        if (string.IsNullOrWhiteSpace(userId))
        {
            return Result<GetBookingDto>.Failure(new Error(ErrorCodes.Failure, "User are not Authorized"));
        }

        // validate dto by business rules

        var nights = createBookingDto.CheckOut.DayNumber - createBookingDto.CheckIn.DayNumber;
        if (nights <= 0)
        {
            return Result<GetBookingDto>.Failure(new Error(ErrorCodes.Failure, "Days are not correct"));
        }
        var hotel = await context.Hotels.FirstOrDefaultAsync(h=>h.Id==createBookingDto.HotelId);
        if (hotel is null)
            return Result<GetBookingDto>.Failure(new Error(ErrorCodes.Failure, $"This hotel id {createBookingDto.HotelId} is not exist  "));
        if (createBookingDto.Guests <= 0)

            return Result<GetBookingDto>.Failure(new Error(ErrorCodes.Failure, "gusets must be more than 0 "));

        var totalPrice = hotel.PerNightRate * nights;

        var booking = new Booking
        {
            HotelId = createBookingDto.HotelId,
            UserId = userId,
            CheckIn = createBookingDto.CheckIn,
            CheckOut = createBookingDto.CheckOut,
            CreatedAtUtc = DateTime.UtcNow,
            Guests = createBookingDto.Guests,
            TotalPrice = totalPrice,
            Status = BookingStatusEnum.Pending,

        };
        try
        {
            context.Bookings.Add(booking);
            await context.SaveChangesAsync();

        }
        catch (Exception e)
        {
            return Result<GetBookingDto>.Failure(new Error(ErrorCodes.Failure, e.Message));
        }

        var dto = new GetBookingDto
        {
            HotelName = hotel.Name,
            CheckIn = booking.CheckIn,
            CheckOut = booking.CheckOut,
            CreatedAtUtc = booking.CreatedAtUtc,
            Guests = booking.Guests,
            HotelId = booking.HotelId,
            Id = booking.Id,
            Status = booking.Status.ToString(),
            TotalPrice = booking.TotalPrice,
            UpdatedAtUtc = booking.UpdatedAtUtc
        };
        return Result<GetBookingDto>.Success(dto);

    }

    public async Task<Result<GetBookingDto>> UpdateBookingAsync(int hotelId, int bookingId, UpdateBookingDto updateBookingDto)
    {
        var userId = authService.UserId;

        if (string.IsNullOrWhiteSpace(userId))
        {
            return Result<GetBookingDto>.Failure(new Error(ErrorCodes.Failure, "User are not Authorized"));
        }


        var nights = updateBookingDto.CheckOut.DayNumber - updateBookingDto.CheckIn.DayNumber;
        if (nights <= 0)
        {
            return Result<GetBookingDto>.Failure(new Error(ErrorCodes.Failure, "Days are not correct"));
        }
        var hotel = await context.Hotels.FirstOrDefaultAsync(h=>h.Id==hotelId);
        if (hotel is null)
            return Result<GetBookingDto>.Failure(new Error(ErrorCodes.Failure, $"This hotel id {hotelId} is not exist  "));

        // we already validate it on dto
        /*if (updateBookingDto.Guests <= 0)

            return Result<GetBookingDto>.Failure(new Error(ErrorCodes.Failure, "gusets must be more than 0 "));*/

        // check if this booking exist 
        var booking= await context.Bookings.Include(b=>b.Hotel).FirstOrDefaultAsync(
            q=>q.HotelId==hotelId
            &&q.Id==bookingId
            && q.UserId==userId
            
            );
        if (booking is null)
            return Result<GetBookingDto>.NotFound(new Error(ErrorCodes.NotFound, "this booking is not exist"));
        if (booking.Status == BookingStatusEnum.Cancelled) 
            return Result<GetBookingDto>.Failure(new Error(ErrorCodes.Failure, "Cancelld booking can't modified "));

        // here get per night rate

        var perNightRate = booking.Hotel!.PerNightRate;
        booking.CheckIn = updateBookingDto.CheckIn;
        booking.CheckOut = updateBookingDto.CheckOut;
        booking.Guests = updateBookingDto.Guests;
        booking.TotalPrice = perNightRate * (updateBookingDto.CheckOut.DayNumber - updateBookingDto.CheckIn.DayNumber);
        booking.UpdatedAtUtc= DateTime.UtcNow;

        await context.SaveChangesAsync();

        var dto = new GetBookingDto
        {
            HotelName = hotel.Name,
            CheckIn = booking.CheckIn,
            CheckOut = booking.CheckOut,
            CreatedAtUtc = booking.CreatedAtUtc,
            Guests = booking.Guests,
            HotelId = booking.HotelId,
            Id = booking.Id,
            Status = booking.Status.ToString(),
            TotalPrice = booking.TotalPrice,
            UpdatedAtUtc = booking.UpdatedAtUtc
        };
        return Result<GetBookingDto>.Success(dto);

    }

    public async Task<Result> CancelBookingAsync(int hotelId, int bookingId)
    {
        var userId = authService.UserId;

        if (string.IsNullOrWhiteSpace(userId))
        {
            return Result.Failure(new Error(ErrorCodes.Failure, "User are not Authorized"));
        }
        var hotel = await context.Hotels.FirstOrDefaultAsync(h => h.Id == hotelId);
        if (hotel is null)
            return Result.Failure(new Error(ErrorCodes.Failure, $"This hotel id {hotelId} is not exist  "));

        var booking = await context.Bookings.Include(b => b.Hotel).FirstOrDefaultAsync(
         q => q.HotelId == hotelId
         && q.Id == bookingId
         && q.UserId == userId

         );
        if (booking is null)
            return Result.NotFound(new Error(ErrorCodes.NotFound, "this booking is not exist"));
        if (booking.Status == BookingStatusEnum.Cancelled)
            return Result.Failure(new Error(ErrorCodes.Failure, "Cancelld booking can't modified "));

        booking.Status = BookingStatusEnum.Cancelled;

        await context.SaveChangesAsync();
        return Result.Success();

    }

    public async Task<Result> AdminConfirmBookingAsync(int hotelId, int bookingId)
    {
        var userId = authService.UserId;

        var booking = await context.Bookings
            .Include(b => b.Hotel)
            .FirstOrDefaultAsync(b =>
                b.Id == bookingId
                && b.HotelId == hotelId);

        if (booking is null)
            return Result.Failure(new Error(ErrorCodes.NotFound, $"Booking '{bookingId}' was not found."));

        if (booking.Status == BookingStatusEnum.Cancelled)
            return Result.Failure(new Error(ErrorCodes.Conflict, "This booking has already been cancelled."));

        booking.Status = BookingStatusEnum.Confirmed;
        booking.UpdatedAtUtc = DateTime.UtcNow;
        await context.SaveChangesAsync();

        return Result.Success();
    }

}
