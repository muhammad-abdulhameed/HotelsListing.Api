using HotelListing.Api.Constants;
using HotelListing.Api.Results;
using HotelsListing.Api.AuthrizationFilters;
using HotelsListing.Api.Contructs;
using HotelsListing.Api.Dtos.Bookings;
using HotelsListing.common.Models.Paging;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;

namespace HotelsListing.Api.Controllers;

[Route("api/hotels/{hotelId:int}/bookings")]
[ApiController]
public class HotelBookingController(IBookingService bookingService) : BaseApiController
{
    [HttpGet]
    public async Task<ActionResult<PagedResult<GetBookingDto>>> GetBookings([FromRoute] int id, [FromQuery] PaginationParameters paginationParameters)
    {
        var result = await bookingService.GetBookingsAsync(id,paginationParameters);
        return ToActionResult(result);
    }
    [HttpPost]
    public async Task<ActionResult<GetBookingDto>> CreatePost([FromRoute] int id, [FromBody] CreateBookingDto dto)
    {
        var result = await bookingService.CreateBookingAsync(dto);

        return ToActionResult(result);
    }
    [HttpPut("{bookingId: int}")]
    [HotelOrSystemAdmin]
    public async Task<ActionResult<GetBookingDto>> UpdateBooking
        (
        [FromRoute ] int hotelId,
        [FromRoute ] int bookingId,
        [FromBody ] UpdateBookingDto updateBookingDto
        ) 
    {
        var result = await bookingService.UpdateBookingAsync(hotelId, bookingId, updateBookingDto);
        return ToActionResult(result);
    }

    [HttpPut("cancel/{bookingId: int}")]
    [HotelOrSystemAdmin]

    public async Task<ActionResult<GetBookingDto>> CancelBooking
     (
     [FromRoute] int hotelId,
     [FromRoute] int bookingId
     
     )
    {
        var result = await bookingService.CancelBookingAsync(hotelId, bookingId);
        return ToActionResult(result);
    }


}
