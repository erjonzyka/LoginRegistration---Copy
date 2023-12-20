#pragma warning disable CS8618
using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
namespace LoginRegistration.Models;
public class Association
{
    [Key]    
    public int AssociationId { get; set; } 
       
    public int? GuestId { get; set; }    
    public int? WeddingId { get; set; }
    // Our navigation properties - don't forget the ?    
    public UserReg? Guest { get; set; }    
    public Wedding? Wedding { get; set; }
}
