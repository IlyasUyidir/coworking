using Coworking.Services;
using Coworking.Utilities;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Coworking.Controllers;

/// <summary>
/// Controller for admin dashboard
/// </summary>
[Authorize(Roles = Constants.Roles.Administrator)]
public class DashboardController : Controller
{
    private readonly IDashboardService _dashboardService;
    private readonly IBookingService _bookingService;
    private readonly ILogger<DashboardController> _logger;

    public DashboardController(
        IDashboardService dashboardService,
        IBookingService bookingService,
        ILogger<DashboardController> logger)
    {
        _dashboardService = dashboardService;
        _bookingService = bookingService;
        _logger = logger;
    }

    /// <summary>
    /// Dashboard home
    /// </summary>
    [HttpGet]
    public async Task<IActionResult> Index()
    {
        var dashboardData = await _dashboardService.GetDashboardDataAsync();
        return View(dashboardData);
    }

    /// <summary>
    /// Master calendar view
    /// </summary>
    [HttpGet]
    public async Task<IActionResult> Calendar()
    {
        var bookings = await _bookingService.GetAllBookingsAsync();
        return View(bookings);
    }

    /// <summary>
    /// Revenue dashboard
    /// </summary>
    [HttpGet]
    public async Task<IActionResult> Revenue(int? year)
    {
        var selectedYear = year ?? DateTime.Now.Year;
        
        var totalRevenue = await _dashboardService.GetTotalRevenueAsync();
        var yearlyRevenue = await _dashboardService.GetTotalRevenueAsync(
            new DateTime(selectedYear, 1, 1),
            new DateTime(selectedYear + 1, 1, 1));
        
        var revenueByMonth = await _dashboardService.GetRevenueByMonthAsync(selectedYear);
        var bookingStats = await _dashboardService.GetBookingStatisticsAsync();
        
        ViewBag.SelectedYear = selectedYear;
        ViewBag.TotalRevenue = totalRevenue;
        ViewBag.YearlyRevenue = yearlyRevenue;
        ViewBag.RevenueByMonth = revenueByMonth;
        ViewBag.BookingStats = bookingStats;
        
        return View();
    }

    /// <summary>
    /// Get calendar events (AJAX)
    /// </summary>
    [HttpGet]
    public async Task<IActionResult> GetCalendarEvents(DateTime start, DateTime end)
    {
        var bookings = await _bookingService.GetAllBookingsAsync();
        
        var events = bookings
            .Where(b => b.StartTime >= start && b.EndTime <= end)
            .Select(b => new
            {
                id = b.Id,
                title = $"{b.Room.Name} - {b.User.FullName}",
                start = b.StartTime.ToString("yyyy-MM-ddTHH:mm:ss"),
                end = b.EndTime.ToString("yyyy-MM-ddTHH:mm:ss"),
                color = b.Status switch
                {
                    Models.BookingStatus.Confirmed => "#28a745",
                    Models.BookingStatus.Pending => "#ffc107",
                    Models.BookingStatus.Cancelled => "#dc3545",
                    Models.BookingStatus.Completed => "#6c757d",
                    _ => "#007bff"
                },
                extendedProps = new
                {
                    roomName = b.Room.Name,
                    userName = b.User.FullName,
                    userEmail = b.User.Email,
                    status = b.Status.ToString(),
                    totalPrice = b.TotalPrice
                }
            })
            .ToList();

        return Json(events);
    }
}
