using System.ComponentModel.DataAnnotations;

namespace Coworking.Models;

/// <summary>
/// Represents an amenity that can be associated with rooms
/// </summary>
public class Amenity
{
    /// <summary>
    /// Unique identifier for the amenity
    /// </summary>
    [Key]
    public int Id { get; set; }
    
    /// <summary>
    /// Name of the amenity (e.g., "WiFi", "Projector", "Whiteboard")
    /// </summary>
    [Required]
    [StringLength(50)]
    public string Name { get; set; } = string.Empty;
    
    /// <summary>
    /// CSS class for icon display (e.g., "fa-wifi" for Font Awesome icons)
    /// </summary>
    [StringLength(50)]
    [Display(Name = "Icon CSS Class")]
    public string IconCssClass { get; set; } = string.Empty;
    
    /// <summary>
    /// Navigation property for rooms that have this amenity
    /// </summary>
    public virtual ICollection<RoomAmenity> RoomAmenities { get; set; } = new List<RoomAmenity>();
}
