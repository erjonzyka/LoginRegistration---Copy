#pragma warning disable CS8618
using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
namespace LoginRegistration.Models;

public class UserLogin
{

    [Required(ErrorMessage ="Email Adress is required")]
    [EmailAddress]
    [ExistingEmail]

    public string LEmail { get; set; }
    [Required]
    [MinLength(8, ErrorMessage ="Password must be at least 8 characters")]
    public string LPassword { get; set; }


}



public class ExistingEmailAttribute : ValidationAttribute
{
    protected override ValidationResult? IsValid(object? value, ValidationContext validationContext)
    {

        if (value == null)
        {

            return new ValidationResult("Email is required!");
        }


        MyContext _context = (MyContext)validationContext.GetService(typeof(MyContext));

        if (!_context.Users.Any(e => e.Email == value.ToString()))
        {

            return new ValidationResult("User not registered");
        }
        else
        {

            return ValidationResult.Success;
        }
    }
}




