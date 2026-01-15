using Coworking.Data;
using Coworking.Models;
using Coworking.ViewModels;
using Microsoft.EntityFrameworkCore;

namespace Coworking.Services;

/// <summary>
/// Implementation of dashboard service
/// </summary>
public class DashboardService : IDashboardService
{
    private readonly ApplicationDbContext _context;
    private readonly ILogger<DashboardService> _logger;

    public DashboardService(ApplicationDbContext context, ILogger<DashboardService> logger)
    {
        _context = context;
        _logger = logger;
    }

    public async Task<DashboardViewModel> GetDashboardDataAsync()
    {
        var now = DateTime.UtcNow;
        var startOfMonth = new DateTime(now.Year, now.Month, 1);
        var endOfMonth = startOfMonth.AddMonths(1);
        var startOfYear = new DateTime(now.Year, 1, 1);
        var endOfYear = new DateTime(now.Year + 1, 1, 1);

        var totalRevenue = await GetTotalRevenueAsync();
        var monthlyRevenue = await GetTotalRevenueAsync(startOfMonth, endOfMonth);
        var yearlyRevenue = await GetTotalRevenueAsync(startOfYear, endOfYear);
        
        var totalBookings = await _context.Bookings.CountAsync();
        var activeBookings = await _context.Bookings
            .CountAsync(b => b.Status == BookingStatus.Confirmed && b.EndTime > now);
        var pendingBookings = await _context.Bookings
            .CountAsync(b => b.Status == BookingStatus.Pending);
        var completedBookings = await _context.Bookings
            .CountAsync(b => b.Status == BookingStatus.Completed);
        
        var upcomingBookings = await _context.Bookings
            .Include(b => b.Room)
            .Include(b => b.User)
            .Where(b => b.Status == BookingStatus.Confirmed && b.StartTime > now)
            .OrderBy(b => b.StartTime)
            .Take(10)
            .ToListAsync();

        var recentBookings = await _context.Bookings
            .Include(b => b.Room)
            .Include(b => b.User)
            .OrderByDescending(b => b.CreatedAt)
            .Take(10)
            .ToListAsync();

        var popularRooms = await GetMostPopularRoomsAsync();
        var bookingStats = await GetBookingStatisticsAsync();
        var revenueByMonth = await GetRevenueByMonthAsync(now.Year);
        
        var totalRooms = await _context.Rooms.CountAsync();
        var operationalRooms = await _context.Rooms.CountAsync(r => r.IsOperational);

        return new DashboardViewModel
        {
            TotalRevenue = totalRevenue,
            MonthlyRevenue = monthlyRevenue,
            YearlyRevenue = yearlyRevenue,
            TotalBookings = totalBookings,
            ActiveBookings = activeBookings,
            PendingBookings = pendingBookings,
            CompletedBookings = completedBookings,
            UpcomingBookings = upcomingBookings,
            RecentBookings = recentBookings,
            PopularRooms = popularRooms,
            BookingsByStatus = bookingStats,
            TotalRooms = totalRooms,
            OperationalRooms = operationalRooms
        };
    }

    public async Task<decimal> GetTotalRevenueAsync(DateTime? startDate = null, DateTime? endDate = null)
    {
        var query = _context.Bookings
            .Where(b => b.Status == BookingStatus.Confirmed || b.Status == BookingStatus.Completed);

        if (startDate.HasValue)
        {
            query = query.Where(b => b.StartTime >= startDate.Value);
        }

        if (endDate.HasValue)
        {
            query = query.Where(b => b.StartTime < endDate.Value);
        }

        return await query.SumAsync(b => (decimal?)b.TotalPrice) ?? 0;
    }

    public async Task<Dictionary<string, int>> GetBookingStatisticsAsync()
    {
        var statistics = new Dictionary<string, int>
        {
            ["Total"] = await _context.Bookings.CountAsync(),
            ["Pending"] = await _context.Bookings.CountAsync(b => b.Status == BookingStatus.Pending),
            ["Confirmed"] = await _context.Bookings.CountAsync(b => b.Status == BookingStatus.Confirmed),
            ["Cancelled"] = await _context.Bookings.CountAsync(b => b.Status == BookingStatus.Cancelled),
            ["Completed"] = await _context.Bookings.CountAsync(b => b.Status == BookingStatus.Completed)
        };

        return statistics;
    }

    public async Task<Dictionary<string, decimal>> GetRevenueByMonthAsync(int year)
    {
        var revenueByMonth = new Dictionary<string, decimal>();
        var monthNames = new[] { "Jan", "Feb", "Mar", "Apr", "May", "Jun", "Jul", "Aug", "Sep", "Oct", "Nov", "Dec" };

        for (int month = 1; month <= 12; month++)
        {
            var startDate = new DateTime(year, month, 1);
            var endDate = startDate.AddMonths(1);

            var revenue = await _context.Bookings
                .Where(b => (b.Status == BookingStatus.Confirmed || b.Status == BookingStatus.Completed) &&
                           b.StartTime >= startDate &&
                           b.StartTime < endDate)
                .SumAsync(b => (decimal?)b.TotalPrice) ?? 0;

            revenueByMonth[monthNames[month - 1]] = revenue;
        }

        return revenueByMonth;
    }

    public async Task<List<(string RoomName, int BookingCount)>> GetMostPopularRoomsAsync(int topCount = 5)
    {
        var popularRooms = await _context.Bookings
            .Where(b => b.Status != BookingStatus.Cancelled)
            .GroupBy(b => new { b.RoomId, b.Room.Name })
            .Select(g => new
            {
                RoomName = g.Key.Name,
                BookingCount = g.Count()
            })
            .OrderByDescending(x => x.BookingCount)
            .Take(topCount)
            .ToListAsync();

        return popularRooms.Select(x => (x.RoomName, x.BookingCount)).ToList();
    }
}
