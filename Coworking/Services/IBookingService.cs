using Coworking.Models;
using Coworking.ViewModels;

namespace Coworking.Services;

/// <summary>
/// Interface for booking-related operations
/// </summary>
public interface IBookingService
{
    /// <summary>
    /// Check if a room is available for the specified time slot
    /// </summary>
    Task<bool> IsRoomAvailableAsync(int roomId, DateTime startTime, DateTime endTime, int? excludeBookingId = null);
    
    /// <summary>
    /// Calculate the total price for a booking
    /// </summary>
    Task<decimal> CalculatePriceAsync(int roomId, DateTime startTime, DateTime endTime);
    
    /// <summary>
    /// Create a new booking
    /// </summary>
    Task<Booking> CreateBookingAsync(BookingViewModel model, string userId);
    
    /// <summary>
    /// Get all bookings for a specific user
    /// </summary>
    Task<List<Booking>> GetUserBookingsAsync(string userId);
    
    /// <summary>
    /// Get a booking by ID
    /// </summary>
    Task<Booking?> GetBookingByIdAsync(int id);
    
    /// <summary>
    /// Cancel a booking
    /// </summary>
    Task<bool> CancelBookingAsync(int id, string userId);
    
    /// <summary>
    /// Get all bookings (admin only)
    /// </summary>
    Task<List<Booking>> GetAllBookingsAsync();
    
    /// <summary>
    /// Update booking status
    /// </summary>
    Task<bool> UpdateBookingStatusAsync(int id, BookingStatus status);
}
