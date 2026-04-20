using HotelListing.Api.Results;
using HotelsListing.Api.Dtos.HotelDtos;
using HotelsListing.common.Models.Filtering;
using HotelsListing.common.Models.Paging;

namespace HotelsListing.Api.Contructs
{
    public interface IHotelService
    {
        public Task<Result<PagedResult<GetHotelsDto>>> GetHotels(PaginationParameters paginationParameters, HotelFilterParameters filters);
        public Task<Result<GetHotelDto>> GetHotel(int id);
        public Task<Result> PutHotel(int id, UpdateHotelDto updateHotel);
        public Task<Result<GetHotelDto>> PostHotel(PostHotelDto HotelPostDto);
        public Task DeleteHotel(int id);
        public Task<bool> HotelExistsAsync(int id);

    }
}
