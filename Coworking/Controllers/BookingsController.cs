using Coworking.Models;
using Coworking.Services;
using Coworking.ViewModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace Coworking.Controllers;

/// <summary>
/// Controller for booking management
/// </summary>
[Authorize]
public class BookingsController : Controller
{
    private readonly IBookingService _bookingService;
    private readonly IRoomService _roomService;
    private readonly UserManager<ApplicationUser> _userManager;
    private readonly ILogger<BookingsController> _logger;

    public BookingsController(
        IBookingService bookingService,
        IRoomService roomService,
        UserManager<ApplicationUser> userManager,
        ILogger<BookingsController> logger)
    {
        _bookingService = bookingService;
        _roomService = roomService;
        _userManager = userManager;
        _logger = logger;
    }

    /// <summary>
    /// Create booking form
    /// </summary>
    [HttpGet]
    public async Task<IActionResult> Create(int? roomId)
    {
        var rooms = await _roomService.GetAllRoomsAsync();
        
        var model = new BookingViewModel
        {
            AvailableRooms = rooms,
            RoomId = roomId ?? 0,
            StartTime = DateTime.Now.AddHours(1),
            EndTime = DateTime.Now.AddHours(2)
        };

        return View(model);
    }

    /// <summary>
    /// Create booking
    /// </summary>
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Create(BookingViewModel model)
    {
        if (ModelState.IsValid)
        {
            try
            {
                var userId = User.FindFirstValue(ClaimTypes.NameIdentifier)!;
                
                // Validate time range
                if (model.EndTime <= model.StartTime)
                {
                    ModelState.AddModelError("", "End time must be after start time.");
                    model.AvailableRooms = await _roomService.GetAllRoomsAsync();
                    return View(model);
                }

                // Check availability
                var isAvailable = await _bookingService.IsRoomAvailableAsync(
                    model.RoomId, model.StartTime, model.EndTime);

                if (!isAvailable)
                {
                    ModelState.AddModelError("", "This room is not available for the selected time slot.");
                    model.AvailableRooms = await _roomService.GetAllRoomsAsync();
                    return View(model);
                }

                var booking = await _bookingService.CreateBookingAsync(model, userId);
                
                TempData["Success"] = $"Booking created successfully! Total: ${booking.TotalPrice:F2}";
                return RedirectToAction(nameof(MyBookings));
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error creating booking");
                ModelState.AddModelError("", ex.Message);
            }
        }

        model.AvailableRooms = await _roomService.GetAllRoomsAsync();
        return View(model);
    }

    /// <summary>
    /// View user's bookings
    /// </summary>
    [HttpGet]
    public async Task<IActionResult> MyBookings()
    {
        var userId = User.FindFirstValue(ClaimTypes.NameIdentifier)!;
        var bookings = await _bookingService.GetUserBookingsAsync(userId);
        
        return View(bookings);
    }

    /// <summary>
    /// View booking details
    /// </summary>
    [HttpGet]
    public async Task<IActionResult> Details(int id)
    {
        var booking = await _bookingService.GetBookingByIdAsync(id);
        
        if (booking == null)
        {
            return NotFound();
        }

        var userId = User.FindFirstValue(ClaimTypes.NameIdentifier)!;
        
        // Check if user owns this booking or is admin
        if (booking.UserId != userId && !User.IsInRole("Administrator"))
        {
            return Forbid();
        }

        return View(booking);
    }

    /// <summary>
    /// Cancel booking
    /// </summary>
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Cancel(int id)
    {
        var userId = User.FindFirstValue(ClaimTypes.NameIdentifier)!;
        var result = await _bookingService.CancelBookingAsync(id, userId);
        
        if (result)
        {
            TempData["Success"] = "Booking cancelled successfully.";
        }
        else
        {
            TempData["Error"] = "Unable to cancel booking.";
        }

        return RedirectToAction(nameof(MyBookings));
    }

    /// <summary>
    /// Check room availability (AJAX)
    /// </summary>
    [HttpPost]
    public async Task<IActionResult> CheckAvailability(int roomId, DateTime startTime, DateTime endTime)
    {
        var isAvailable = await _bookingService.IsRoomAvailableAsync(roomId, startTime, endTime);
        var price = isAvailable ? await _bookingService.CalculatePriceAsync(roomId, startTime, endTime) : 0;
        
        return Json(new
        {
            available = isAvailable,
            price = price,
            message = isAvailable 
                ? $"Room is available. Total price: ${price:F2}" 
                : "Room is not available for this time slot."
        });
    }
}
