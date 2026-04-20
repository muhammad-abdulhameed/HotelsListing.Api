using System.ComponentModel.DataAnnotations;

namespace HotelsListing.Api.Dtos.Bookings;

public class CreateBookingDto: IValidatableObject
{
    [Required]
    public int HotelId { get; set; }
    [Range(1, 30)]
    public int Guests { get; set; }
    public DateOnly CheckIn { get; set; }
    public DateOnly CheckOut { get; set; }
    public IEnumerable<ValidationResult> Validate(ValidationContext validationContext)
    {
        if (CheckOut < CheckIn)
        {
            yield return new ValidationResult("chickout must be grater tthan chcckin", [nameof(CheckOut), nameof(CheckIn)]);
        }
    }
}
