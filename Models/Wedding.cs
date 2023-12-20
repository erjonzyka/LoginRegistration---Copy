#pragma warning disable CS8618
using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
namespace LoginRegistration.Models;

public class Wedding
{

    [Key]
    public int WeddingId { get; set; }
    [Required(ErrorMessage ="Name is required")]
    [MinLength(2, ErrorMessage ="First Name must be at least 2 characters")]
    public string WedderOne { get; set; }

    [Required(ErrorMessage ="Name is required")]
    [MinLength(2, ErrorMessage ="First Name must be at least 2 characters")]
    public string WedderTwo { get; set; }
    [Required(ErrorMessage = "Date is Required")]
    [FutureDate(ErrorMessage = "Please enter a future date.")]
    public DateTime WeddingDate {get;set;}

    [Required(ErrorMessage ="Adress is required")]
    [MinLength(2, ErrorMessage ="Adress must be at least 2 characters")]
    public string Adress {get;set;}

    public int CreatorId {get;set;}
    public UserReg? Creator { get; set; }    
    public List<Association> Associations { get; set; } = new List<Association>();
}


public class FutureDateAttribute : ValidationAttribute
{
    public override bool IsValid(object value)
    {
        if (value is DateTime date)
        {
            return date > DateTime.Now;
        }
        return false;
    }
}