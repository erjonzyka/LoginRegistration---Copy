#pragma warning disable CS8618
using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
namespace LoginRegistration.Models;

public class UserReg
{

    [Key]
    public int id { get; set; }
    [Required(ErrorMessage ="Name is required")]
    [MinLength(2, ErrorMessage ="First Name must be at least 2 characters")]
    public string FirstName { get; set; }
    [Required(ErrorMessage ="Last Name is required")]
    [MinLength(2, ErrorMessage ="Last Name must be at least 2 characters")]
    public string LastName { get; set; }
    [Required(ErrorMessage ="Email Adress is required")]
    [EmailAddress]
    [UniqueEmail]
    public string Email { get; set; }
    [Required]
    [MinLength(8, ErrorMessage ="Password must be at least 8 characters")]
    public string Password { get; set; }
    [NotMapped]
    [Required]
    [MinLength(8)]
    [Compare("Password",ErrorMessage ="Passwords must match!")]
    public string Pwconfirm { get; set; }

    public DateTime CreatedAt { get; set; } = DateTime.Now;
    public DateTime UpdatedAt { get; set; } = DateTime.Now;
    public List<Wedding> Weddings { get; set; } = new List<Wedding>();
    public List<Association> Associations { get; set; } = new List<Association>();
}



public class UniqueEmailAttribute : ValidationAttribute
{
    protected override ValidationResult? IsValid(object? value, ValidationContext validationContext)
    {

        if (value == null)
        {

            return new ValidationResult("Email is required!");
        }


        MyContext _context = (MyContext)validationContext.GetService(typeof(MyContext));

        if (_context.Users.Any(e => e.Email == value.ToString()))
        {

            return new ValidationResult("Email must be unique!");
        }
        else
        {

            return ValidationResult.Success;
        }
    }
}




