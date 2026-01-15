using Coworking.Models;

namespace Coworking.ViewModels;

/// <summary>
/// View model for room display with amenities
/// </summary>
public class RoomViewModel
{
    public int Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public int Capacity { get; set; }
    public decimal PricePerHour { get; set; }
    public string Description { get; set; } = string.Empty;
    public string ImageUrl { get; set; } = string.Empty;
    public bool IsOperational { get; set; }
    
    // Amenities for this room
    public List<Amenity> Amenities { get; set; } = new List<Amenity>();
    
    // All available amenities (for edit form)
    public List<Amenity>? AllAmenities { get; set; }
    
    // Selected amenity IDs (for form binding)
    public List<int> SelectedAmenityIds { get; set; } = new List<int>();
    
    // Booking information
    public int TotalBookings { get; set; }
    public DateTime? NextAvailableSlot { get; set; }
}
