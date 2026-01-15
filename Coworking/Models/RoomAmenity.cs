namespace Coworking.Models;

/// <summary>
/// Junction table for many-to-many relationship between Room and Amenity
/// </summary>
public class RoomAmenity
{
    /// <summary>
    /// Foreign key to Room
    /// </summary>
    public int RoomId { get; set; }
    
    /// <summary>
    /// Navigation property to Room
    /// </summary>
    public virtual Room Room { get; set; } = null!;
    
    /// <summary>
    /// Foreign key to Amenity
    /// </summary>
    public int AmenityId { get; set; }
    
    /// <summary>
    /// Navigation property to Amenity
    /// </summary>
    public virtual Amenity Amenity { get; set; } = null!;
}
