using Microsoft.AspNetCore.Mvc.DataAnnotations;
using System.ComponentModel.DataAnnotations;

namespace companyApi.Validators
{
    public class DateCheckAtrribute : ValidationAttribute
    {
        protected override ValidationResult? IsValid(object? value, ValidationContext validationContext)
        {
            var date = (DateTime?)value;
            if(date < DateTime.Today)
            {
                return new ValidationResult("Hire Date must be greater than or equal to today's dates");
            }

            return ValidationResult.Success;
        }
    }

}
