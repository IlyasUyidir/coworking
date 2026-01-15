using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Coworking.Models;

/// <summary>
/// Represents a coworking space room available for booking
/// </summary>
public class Room
{
    /// <summary>
    /// Unique identifier for the room
    /// </summary>
    [Key]
    public int Id { get; set; }
    
    /// <summary>
    /// Name of the room (e.g., "Conference Room A", "Private Office 1")
    /// </summary>
    [Required]
    [StringLength(100)]
    public string Name { get; set; } = string.Empty;
    
    /// <summary>
    /// Maximum capacity (number of people)
    /// </summary>
    [Required]
    [Range(1, 100)]
    public int Capacity { get; set; }
    
    /// <summary>
    /// Price per hour for booking this room
    /// </summary>
    [Required]
    [Column(TypeName = "decimal(18,2)")]
    [Range(0.01, 10000.00)]
    [Display(Name = "Price Per Hour")]
    public decimal PricePerHour { get; set; }
    
    /// <summary>
    /// Detailed description of the room
    /// </summary>
    [StringLength(500)]
    public string Description { get; set; } = string.Empty;
    
    /// <summary>
    /// URL or path to the room's image
    /// </summary>
    [StringLength(255)]
    [Display(Name = "Image URL")]
    public string ImageUrl { get; set; } = string.Empty;
    
    /// <summary>
    /// Indicates whether the room is currently operational and available for booking
    /// </summary>
    [Display(Name = "Is Operational")]
    public bool IsOperational { get; set; } = true;
    
    /// <summary>
    /// Navigation property for bookings of this room
    /// </summary>
    public virtual ICollection<Booking> Bookings { get; set; } = new List<Booking>();
    
    /// <summary>
    /// Navigation property for room amenities (many-to-many)
    /// </summary>
    public virtual ICollection<RoomAmenity> RoomAmenities { get; set; } = new List<RoomAmenity>();
}
