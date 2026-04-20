using AutoMapper;
using AutoMapper.QueryableExtensions;
using HotelListing.Api.Results;
using HotelsListing.Api.Contructs;
using HotelsListing.Api.Data;
using HotelsListing.Api.DomainObj;
using HotelsListing.Api.Dtos.HotelDtos;
using HotelsListing.common.Extensions;
using HotelsListing.common.Models.Filtering;
using HotelsListing.common.Models.Paging;
using Humanizer;
using Microsoft.EntityFrameworkCore;
using static NuGet.Packaging.PackagingConstants;

namespace HotelsListing.Api.services
{                                                                   // that's obj of auto map that we created on ioc container
    public class HotelService(HotelsListingApiDataContext context,IMapper mapper) : IHotelService
    {
       

        public async Task<Result<PagedResult<GetHotelsDto>>> GetHotels(PaginationParameters paginationParameters , HotelFilterParameters filters)
        {
            var query = context.Hotels.AsQueryable();
            if (filters.CountryId.HasValue)
            {
                query = query.Where(q => q.CountryId == filters.CountryId);
            }
            if (filters.MaxPrice.HasValue) 
            {
                query = query.Where(h => h.PerNightRate <= filters.MaxPrice);
            }
            if (!string.IsNullOrWhiteSpace(filters.Search))
                query = query.Where(h => h.Name.Contains(filters.Search) ||
                                        h.Country.Name==filters.Search);

            var hotels = await query.ProjectTo<GetHotelsDto>(mapper.ConfigurationProvider).ToPagedResultAsync(paginationParameters);
            // SELECT * FROM Hotels LEFT JOIN Countries ON Hotels.CountryId = Countries.CountryId
            return Result<PagedResult<GetHotelsDto>>.Success(hotels);
            /*   return await context.Hotels
           //.Include(h => h.Country) // Eager loading the Country navigation property
           .ToListAsync();*/
        }

        public async Task<Result<GetHotelDto>> GetHotel(int id)
        {                                                           ///ask gbt about that
            var hotel = await context.Hotels.Where(q => q.Id == id)
                                                            // that's to get the configration to know how to map this 
            .ProjectTo<GetHotelDto>(mapper.ConfigurationProvider) // Eager loading the Country navigation property*///we comment it cause it use less projectin do the join inside the get only the targted coulmn
           /* .Select(h => new GetHotelDto(h.Id, h.Name, h.Rating, h.CountryId, h.Country!.Name))*///here we comment this becuse auto mapper has extention method do the select internally an map the obj
            .FirstOrDefaultAsync();
            if (hotel is null) 
            {
                return Result<GetHotelDto>.NotFound();
            }
            return Result<GetHotelDto>.Success( hotel);
        }

        public async Task<Result<GetHotelDto>> PostHotel(PostHotelDto HotelPostDto)
        {
            var hotelExist = await HotelExistsAsync(HotelPostDto.CountryId);

            if (!hotelExist) 
            {
                return Result<GetHotelDto>.Failure(new Error { Code = ErrorCodes.Failure, Description = $"$\"Hotel ${HotelPostDto.CountryId} was not found.\"" });
            }

            var isDuplicate = await HotelExistsAsync(HotelPostDto.Name);

            if (hotelExist)
            {
                return Result<GetHotelDto>.Failure(new Error { Code = ErrorCodes.Failure, Description = $"$\"Hotel ${HotelPostDto.Name} already exists in the selected country..\"" });
            }

            var hotel = mapper.Map<Hotel>(HotelPostDto);// that's replace the next line
           /*var hotel= new Hotel { Name = HotelPostDto.Name, Rating = HotelPostDto.Rating, CountryId = HotelPostDto.CountryId };*/

            context.Hotels.Add(hotel);
            await context.SaveChangesAsync();

            return Result<GetHotelDto>.Success(mapper.Map<GetHotelDto>(hotel)) ;
        }

        public async Task<Result> PutHotel(int id, UpdateHotelDto updateHotel)

        {
            if (id != updateHotel.Id)
            {
                return Result.BadRequest(new Error(ErrorCodes.Validation, "Id route value does not match payload Id."));
            }
            var hotel = await context.Hotels.FindAsync(id);
            if (hotel == null)
            {
                return Result.NotFound(new Error(ErrorCodes.NotFound, $"Hotel '{id}' was not found."));
            }

          hotel=  mapper.Map(updateHotel, hotel);
            /*hotel.Name = updateHotel.Name;
            hotel.Rating = updateHotel.Rating;
            hotel.CountryId = updateHotel.CountryId;*/





            context.Entry(hotel).State = EntityState.Modified;



            await context.SaveChangesAsync();

            return Result.Success();
        }

        public async Task DeleteHotel(int id)
        {
            var hotel = await context.Hotels.FindAsync(id);
            if (hotel == null)
            {
                throw new KeyNotFoundException();
            }

            context.Hotels.Remove(hotel);
            await context.SaveChangesAsync();
        }

        public async Task<bool> HotelExistsAsync(int id)
        {
          return  await context.Hotels.AnyAsync(e => e.Id == id);
        }
        public async Task<bool> HotelExistsAsync(string name)
        {
            return await context.Hotels.AnyAsync(e => e.Name == name);
        }
    }
}
