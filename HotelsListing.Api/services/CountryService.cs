using AutoMapper;
using AutoMapper.QueryableExtensions;
using HotelListing.Api.Results;
using HotelsListing.Api.Contructs;
using HotelsListing.Api.Data;
using HotelsListing.Api.DomainObj;
using HotelsListing.Api.Dtos.CountryDtos;
using HotelsListing.Api.Dtos.HotelDtos;
using HotelsListing.common.Extensions;
using HotelsListing.common.Models.Filtering;
using HotelsListing.common.Models.Paging;
using Microsoft.AspNetCore.Mvc;
using Microsoft.CodeAnalysis.Elfie.Serialization;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Caching.Memory;
using static NuGet.Packaging.PackagingConstants;

namespace HotelsListing.Api.services
{
    public class CountryService(HotelsListingApiDataContext context,IMapper mapper,IMemoryCache memoryCache,ILogger<CountryService> logger  ) : ICountryService
    {
        public async Task<Result<PagedResult<GetCountryDto>>> GetCountries(PaginationParameters paginationParameters, CountryFilterParameters filters )
        {
            var query = context.Countries.AsQueryable();

            if (!string.IsNullOrWhiteSpace(filters.Search))
            {
                var term = filters.Search.Trim();
                query = query.Where(c => EF.Functions.Like(c.Name, $"%{term}%")
                );
            }

            var countries= await query.ProjectTo<GetCountryDto>(mapper.ConfigurationProvider).ToPagedResultAsync(paginationParameters);
            return Result<PagedResult<GetCountryDto>>.Success(countries);
        }

        public async Task<Result<GetCountryDto>> GetCountry(int id)
        {
            // declare the key
            var cachKey = $"contury_{id}";

            // make cache hit first 
            if (!memoryCache.TryGetValue(cachKey, out GetCountryDto? country))
            {
              
               

                 country = await context.Countries.Where(q => q.CountryId == id).ProjectTo<GetCountryDto>(mapper.ConfigurationProvider)
           /*.Select(c => new GetCountryDto
           {
               CountryId = c.CountryId,
               Name = c.Name,
               Hotels = c.Hotels!.Select(h => new GetHotelsDto(h.Id, h.Name, h.Rating, h.CountryId))
           })*/ // Eager loading the Hotels navigation property
           .FirstOrDefaultAsync();
                if(country is not null)
                {var memoryOptions = new MemoryCacheEntryOptions()
                    .SetSlidingExpiration(TimeSpan.FromMinutes(15))
                    .SetAbsoluteExpiration(TimeSpan.FromMinutes(10));
                    memoryCache.Set(cachKey, country, memoryOptions);
                }

            }

            return country is null
            ? Result<GetCountryDto>.Failure(new Error(ErrorCodes.NotFound, $"Country '{id}' was not found."))
            : Result<GetCountryDto>.Success(country);



        }

        public async Task<Result<GetCountryDto>> PostCountry(PostCountryDto countryPostDto)
        {

            try {
                var isExist = await CountryExistsAsync(countryPostDto.Name);
                if (isExist) 
                {
                    // use i logger that's integrated with serialog in program.cs
                    logger.LogError("Country '{countryPostDto.Name}' was not found.", []);
                    return Result<GetCountryDto>.Failure(new Error(ErrorCodes.NotFound, $"Country '{countryPostDto.Name}' was not found."));
                }


                var country = mapper.Map<Country>(countryPostDto);
                if (country is null) 
                {
                    Result<GetCountryDto>.BadRequest(new Error(ErrorCodes.BadRequest, $"Country '{countryPostDto.Name}' was not found."));
                }
                context.Countries.Add(country!);
                await context.SaveChangesAsync();
                var dto = mapper.Map<GetCountryDto>(country);
                // invalidating cache to force update 
                memoryCache.Remove($"contury_{dto.CountryId}");

                return Result<GetCountryDto>.Success(dto);
            }
            catch
            {
                return Result<GetCountryDto>.Failure(new Error(ErrorCodes.Failure, "An unexpected error occurred while creating the country."));
            }

            
        }

        public async Task<Result>  PutCountry(int id, UpdateCountryDto updateCountry)
        {
            if (id != updateCountry.CountryId)
            {
                return Result.Failure(new Error(ErrorCodes.NotFound, $"Country '{id}' was not found."));
            }
            var country = await context.Countries.FindAsync(id) ;
            if (!await CountryExistsAsync(id)) 
            {

                return Result.NotFound(new Error(ErrorCodes.NotFound, $"Country '{id}' was not found."));
            }

            context.Entry(country).State = EntityState.Modified;
            country=mapper.Map<Country>(updateCountry);
            /*country.CountryId = id;
            country.Name = updateCountry.Name;
            country.Hotels = null;
            country.CountryId = id;*/
            await context.SaveChangesAsync();
            return Result.Success();
        }

        public async Task DeleteCountry(int id)
        {
            var country = await context.Countries.FindAsync(id);
            if (country == null)
            {
                throw new KeyNotFoundException();
            }

            context.Countries.Remove(country);
            await context.SaveChangesAsync();

        }
        public async Task<bool> CountryExistsAsync(int id)
        {
         return   await context.Countries.AnyAsync(e => e.CountryId == id);
        }
        public async Task<bool> CountryExistsAsync(string name)
        {
            return await context.Countries.AnyAsync(e => e.Name == name);
        }

    } }
