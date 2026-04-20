using HotelListing.Api.Results;
using HotelsListing.Api.Controllers;
using HotelsListing.Api.Contructs;
using HotelsListing.Api.Data;
using HotelsListing.Api.DomainObj;
using HotelsListing.Api.Dtos.HotelDtos;
using HotelsListing.common.Models.Filtering;
using HotelsListing.common.Models.Paging;
using Microsoft.AspNetCore.Mvc;

namespace HotelListing.Api.Controllers;

[Route("api/[controller]")]
[ApiController]
public class HotelsController(IHotelService hotelService) : BaseApiController
{

    // GET: api/Hotels
    [HttpGet]
    public async Task<ActionResult<PagedResult<GetHotelsDto>>> GetHotels([FromQuery] PaginationParameters paginationParameters, HotelFilterParameters filters)
    {
        var hotels=   await  hotelService.GetHotels(paginationParameters,filters);
        return ToActionResult(hotels);
    
    }

    // GET: api/Hotels/5
    [HttpGet("{id}")]
    public async Task<ActionResult<GetHotelDto>> GetHotel(int id)
    {
        // SELECT * FROM Hotels
        // LEFT JOIN Countries ON Hotels.CountryId = Countries.CountryId
        // WHERE Hotels.Id = @id

        var hotel = await hotelService.GetHotel(id); 

      

        return ToActionResult(hotel);
    }

    // PUT: api/Hotels/5
    // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
    [HttpPut("{id}")]
    public async Task<IActionResult> PutHotel(int id,  UpdateHotelDto hotelUpdate)
    {
       
       
         var result=  await hotelService.PutHotel(id, hotelUpdate);
       
        

        return ToActionResult(result);
    }

    // POST: api/Hotels
    // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
    [HttpPost]
    public async Task<ActionResult<Hotel>> PostHotel(PostHotelDto postHotel)
    {
        var result=await hotelService.PostHotel(postHotel);
        if (!result.IsSuccess) return MapErrorsToResponse(result.Errors);

        return CreatedAtAction("GetHotel", new { id = result.Value!.Id }, result.Value);
    }

    // DELETE: api/Hotels/5
    [HttpDelete("{id}")]
    public async Task<IActionResult> DeleteHotel(int id)
    {
       

        return NoContent();
    }

    
}