using Microsoft.AspNetCore.Mvc.ModelBinding.Validation;
using System.ComponentModel.DataAnnotations;

namespace HotelsListing.Api.Dtos.Bookings;

// that's to add custom validation on your fields 
/*
 * ✔ Data Annotations like [Required], [Range] → validate single fields

✔ IValidatableObject → validates the whole object
 */
public class UpdateBookingDto:IValidatableObject
{
    [Range(1,12)]
    public int Guests { get; set; }
    public DateOnly CheckIn { get; set; }
    public DateOnly CheckOut { get; set; }

    public IEnumerable<ValidationResult> Validate(ValidationContext validationContext)
    {
        if (CheckOut < CheckIn) 
        {
            yield return new ValidationResult("chickout must be grater tthan chcckin",[nameof(CheckOut),nameof(CheckIn)]);
        }
    }
}
