using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Identity;

namespace Coworking.Models;

/// <summary>
/// Represents an application user, extending ASP.NET Core Identity
/// </summary>
public class ApplicationUser : IdentityUser
{
    /// <summary>
    /// Full name of the user
    /// </summary>
    [Required]
    [StringLength(100)]
    [Display(Name = "Full Name")]
    public string FullName { get; set; } = string.Empty;
    
    /// <summary>
    /// Company name (optional, for business clients)
    /// </summary>
    [StringLength(100)]
    [Display(Name = "Company Name")]
    public string? CompanyName { get; set; }
    
    /// <summary>
    /// Navigation property for user's bookings
    /// </summary>
    public virtual ICollection<Booking> Bookings { get; set; } = new List<Booking>();
}
