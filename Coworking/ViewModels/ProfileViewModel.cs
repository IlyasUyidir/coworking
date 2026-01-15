using System.ComponentModel.DataAnnotations;

namespace Coworking.ViewModels;

/// <summary>
/// View model for user profile management
/// </summary>
public class ProfileViewModel
{
    [Required]
    [StringLength(100)]
    [Display(Name = "Full Name")]
    public string FullName { get; set; } = string.Empty;
    
    [Required]
    [EmailAddress]
    [Display(Name = "Email")]
    public string Email { get; set; } = string.Empty;
    
    [Phone]
    [Display(Name = "Phone Number")]
    public string? PhoneNumber { get; set; }
    
    [StringLength(100)]
    [Display(Name = "Company Name")]
    public string? CompanyName { get; set; }
    
    // Statistics
    public int TotalBookings { get; set; }
    public decimal TotalSpent { get; set; }
    public DateTime MemberSince { get; set; }
}
