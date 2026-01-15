using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Coworking.Models;

/// <summary>
/// Represents a booking for a coworking space room
/// </summary>
public class Booking
{
    /// <summary>
    /// Unique identifier for the booking
    /// </summary>
    [Key]
    public int Id { get; set; }
    
    /// <summary>
    /// Start date and time of the booking
    /// </summary>
    [Required]
    [Display(Name = "Start Time")]
    public DateTime StartTime { get; set; }
    
    /// <summary>
    /// End date and time of the booking
    /// </summary>
    [Required]
    [Display(Name = "End Time")]
    public DateTime EndTime { get; set; }
    
    /// <summary>
    /// Total price for the booking (calculated based on duration and room price)
    /// </summary>
    [Required]
    [Column(TypeName = "decimal(18,2)")]
    [Display(Name = "Total Price")]
    public decimal TotalPrice { get; set; }
    
    /// <summary>
    /// Current status of the booking
    /// </summary>
    [Required]
    public BookingStatus Status { get; set; } = BookingStatus.Pending;
    
    /// <summary>
    /// Foreign key to the booked room
    /// </summary>
    [Required]
    public int RoomId { get; set; }
    
    /// <summary>
    /// Navigation property to the booked room
    /// </summary>
    [ForeignKey(nameof(RoomId))]
    public virtual Room Room { get; set; } = null!;
    
    /// <summary>
    /// Foreign key to the user who made the booking
    /// </summary>
    [Required]
    public string UserId { get; set; } = string.Empty;
    
    /// <summary>
    /// Navigation property to the user who made the booking
    /// </summary>
    [ForeignKey(nameof(UserId))]
    public virtual ApplicationUser User { get; set; } = null!;
    
    /// <summary>
    /// Date and time when the booking was created
    /// </summary>
    [Display(Name = "Created At")]
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
}
