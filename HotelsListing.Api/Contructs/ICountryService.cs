using HotelListing.Api.Results;
using HotelsListing.Api.DomainObj;
using HotelsListing.Api.Dtos.CountryDtos;
using HotelsListing.common.Models.Filtering;
using HotelsListing.common.Models.Paging;
using Microsoft.AspNetCore.Mvc;

namespace HotelsListing.Api.Contructs
{
    public interface ICountryService
    {
        public Task<Result<PagedResult<GetCountryDto>>> GetCountries(PaginationParameters paginationParameters, CountryFilterParameters filters);
        public Task<Result<GetCountryDto>> GetCountry(int id);
        public Task<Result> PutCountry(int id, UpdateCountryDto updateCountry);
        public Task<Result<GetCountryDto>> PostCountry(PostCountryDto countryPostDto);
        public  Task DeleteCountry(int id);
        public  Task<bool> CountryExistsAsync(int id);



    }
}
