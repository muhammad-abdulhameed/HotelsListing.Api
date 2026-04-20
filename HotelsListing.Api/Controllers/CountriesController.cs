using HotelsListing.Api.Controllers;
using HotelsListing.Api.Contructs;
using HotelsListing.Api.Dtos.CountryDtos;
using HotelsListing.common.Models.Filtering;
using HotelsListing.common.Models.Paging;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Caching.Memory;

namespace HotelListing.Api.Controllers;

[Route("api/[controller]")]
[ApiController]
[Authorize]
public class CountriesController(ICountryService countryService) : BaseApiController
{
    // GET: api/Countries
    [HttpGet]
    public async Task<ActionResult<PagedResult<GetCountryDto>>> GetCountries([FromQuery] PaginationParameters paginationParameters, CountryFilterParameters countryFilter )
    {
        var countries = await countryService.GetCountries(paginationParameters,countryFilter);
        // process countries
        return ToActionResult(countries);
    }

    // GET: api/Countries/5
    [HttpGet("{id}")]
    public async Task<ActionResult<GetCountryDto>> GetCountry(int id)
    {
    

        
      
         var country = await countryService.GetCountry(id);
          

        

        return ToActionResult(country);
    }

    // PUT: api/Countries/5
    // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
    [HttpPut("{id}")]
    public async Task<IActionResult> PutCountry(int id, UpdateCountryDto updateCountry)
    {

       var result= await countryService.PutCountry(id, updateCountry);

        return ToActionResult(result);
    }

    // POST: api/Countries
    // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
    [HttpPost]
    public async Task<ActionResult<GetCountryDto>> PostCountry(PostCountryDto countryPostDto)
    {
        var result = await countryService.PostCountry(countryPostDto);
        if (!result.IsSuccess) return MapErrorsToResponse(result.Errors);
        return CreatedAtAction("GetCountry", new { id = result.Value!.CountryId }, result.Value);
    }

    // DELETE: api/Countries/5
    [HttpDelete("{id}")]
    public async Task<IActionResult> DeleteCountry(int id)
    {

        await countryService.DeleteCountry(id);

        return NoContent();
    }


}