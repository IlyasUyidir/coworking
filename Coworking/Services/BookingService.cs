using Coworking.Data;
using Coworking.Models;
using Coworking.ViewModels;
using Microsoft.EntityFrameworkCore;

namespace Coworking.Services;

/// <summary>
/// Implementation of booking service
/// </summary>
public class BookingService : IBookingService
{
    private readonly ApplicationDbContext _context;
    private readonly ILogger<BookingService> _logger;

    public BookingService(ApplicationDbContext context, ILogger<BookingService> logger)
    {
        _context = context;
        _logger = logger;
    }

    public async Task<bool> IsRoomAvailableAsync(int roomId, DateTime startTime, DateTime endTime, int? excludeBookingId = null)
    {
        var conflictingBookings = await _context.Bookings
            .Where(b => b.RoomId == roomId &&
                       b.Status != BookingStatus.Cancelled &&
                       (excludeBookingId == null || b.Id != excludeBookingId) &&
                       ((startTime >= b.StartTime && startTime < b.EndTime) ||
                        (endTime > b.StartTime && endTime <= b.EndTime) ||
                        (startTime <= b.StartTime && endTime >= b.EndTime)))
            .AnyAsync();

        return !conflictingBookings;
    }

    public async Task<decimal> CalculatePriceAsync(int roomId, DateTime startTime, DateTime endTime)
    {
        var room = await _context.Rooms.FindAsync(roomId);
        if (room == null)
        {
            throw new ArgumentException("Room not found", nameof(roomId));
        }

        var duration = (endTime - startTime).TotalHours;
        return (decimal)duration * room.PricePerHour;
    }

    public async Task<Booking> CreateBookingAsync(BookingViewModel model, string userId)
    {
        // Validate availability
        if (!await IsRoomAvailableAsync(model.RoomId, model.StartTime, model.EndTime))
        {
            throw new InvalidOperationException("Room is not available for the selected time slot.");
        }

        // Calculate price
        var totalPrice = await CalculatePriceAsync(model.RoomId, model.StartTime, model.EndTime);

        var booking = new Booking
        {
            RoomId = model.RoomId,
            UserId = userId,
            StartTime = model.StartTime,
            EndTime = model.EndTime,
            TotalPrice = totalPrice,
            Status = BookingStatus.Confirmed,
            CreatedAt = DateTime.UtcNow
        };

        _context.Bookings.Add(booking);
        await _context.SaveChangesAsync();

        _logger.LogInformation("Booking created: ID {BookingId}, Room {RoomId}, User {UserId}", 
            booking.Id, booking.RoomId, userId);

        return booking;
    }

    public async Task<List<Booking>> GetUserBookingsAsync(string userId)
    {
        return await _context.Bookings
            .Include(b => b.Room)
            .Where(b => b.UserId == userId)
            .OrderByDescending(b => b.StartTime)
            .ToListAsync();
    }

    public async Task<Booking?> GetBookingByIdAsync(int id)
    {
        return await _context.Bookings
            .Include(b => b.Room)
            .Include(b => b.User)
            .FirstOrDefaultAsync(b => b.Id == id);
    }

    public async Task<bool> CancelBookingAsync(int id, string userId)
    {
        var booking = await _context.Bookings.FindAsync(id);
        
        if (booking == null || booking.UserId != userId)
        {
            return false;
        }

        if (booking.Status == BookingStatus.Cancelled || booking.Status == BookingStatus.Completed)
        {
            return false;
        }

        booking.Status = BookingStatus.Cancelled;
        await _context.SaveChangesAsync();

        _logger.LogInformation("Booking cancelled: ID {BookingId}", id);
        return true;
    }

    public async Task<List<Booking>> GetAllBookingsAsync()
    {
        return await _context.Bookings
            .Include(b => b.Room)
            .Include(b => b.User)
            .OrderByDescending(b => b.StartTime)
            .ToListAsync();
    }

    public async Task<bool> UpdateBookingStatusAsync(int id, BookingStatus status)
    {
        var booking = await _context.Bookings.FindAsync(id);
        
        if (booking == null)
        {
            return false;
        }

        booking.Status = status;
        await _context.SaveChangesAsync();

        _logger.LogInformation("Booking status updated: ID {BookingId}, Status {Status}", id, status);
        return true;
    }
}
