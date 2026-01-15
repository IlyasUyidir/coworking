using Coworking.ViewModels;

namespace Coworking.Services;

/// <summary>
/// Interface for dashboard and analytics operations
/// </summary>
public interface IDashboardService
{
    /// <summary>
    /// Get dashboard statistics and data
    /// </summary>
    Task<DashboardViewModel> GetDashboardDataAsync();
    
    /// <summary>
    /// Get total revenue for a date range
    /// </summary>
    Task<decimal> GetTotalRevenueAsync(DateTime? startDate = null, DateTime? endDate = null);
    
    /// <summary>
    /// Get booking statistics
    /// </summary>
    Task<Dictionary<string, int>> GetBookingStatisticsAsync();
    
    /// <summary>
    /// Get revenue by month for the current year
    /// </summary>
    Task<Dictionary<string, decimal>> GetRevenueByMonthAsync(int year);
    
    /// <summary>
    /// Get most popular rooms
    /// </summary>
    Task<List<(string RoomName, int BookingCount)>> GetMostPopularRoomsAsync(int topCount = 5);
}
