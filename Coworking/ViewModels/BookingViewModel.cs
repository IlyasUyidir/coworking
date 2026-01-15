using System.ComponentModel.DataAnnotations;
using Coworking.Models;

namespace Coworking.ViewModels;

/// <summary>
/// View model for booking operations
/// </summary>
public class BookingViewModel
{
    public int Id { get; set; }
    
    [Required]
    [Display(Name = "Room")]
    public int RoomId { get; set; }
    
    [Display(Name = "Room Name")]
    public string RoomName { get; set; } = string.Empty;
    
    [Required]
    [Display(Name = "Start Time")]
    public DateTime StartTime { get; set; } = DateTime.Now.AddHours(1);
    
    [Required]
    [Display(Name = "End Time")]
    public DateTime EndTime { get; set; } = DateTime.Now.AddHours(2);
    
    [Display(Name = "Total Price")]
    public decimal TotalPrice { get; set; }
    
    [Display(Name = "Status")]
    public BookingStatus Status { get; set; }
    
    // For display purposes
    public string UserName { get; set; } = string.Empty;
    public string UserEmail { get; set; } = string.Empty;
    
    // For room selection
    public List<Room>? AvailableRooms { get; set; }
}
