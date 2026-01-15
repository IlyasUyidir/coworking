using Coworking.Models;

namespace Coworking.ViewModels;

/// <summary>
/// View model for admin dashboard
/// </summary>
public class DashboardViewModel
{
    // Revenue metrics
    public decimal TotalRevenue { get; set; }
    public decimal MonthlyRevenue { get; set; }
    public decimal YearlyRevenue { get; set; }
    
    // Booking metrics
    public int TotalBookings { get; set; }
    public int ActiveBookings { get; set; }
    public int PendingBookings { get; set; }
    public int CompletedBookings { get; set; }
    
    // Lists
    public List<Booking> UpcomingBookings { get; set; } = new List<Booking>();
    public List<Booking> RecentBookings { get; set; } = new List<Booking>();
    
    // Analytics
    public List<(string RoomName, int BookingCount)> PopularRooms { get; set; } = new List<(string, int)>();
    public Dictionary<string, decimal> RevenueByMonth { get; set; } = new Dictionary<string, decimal>();
    public Dictionary<string, int> BookingsByStatus { get; set; } = new Dictionary<string, int>();
    
    // Room statistics
    public int TotalRooms { get; set; }
    public int OperationalRooms { get; set; }
}
