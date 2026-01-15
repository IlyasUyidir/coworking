namespace Coworking.Models;

/// <summary>
/// Represents the status of a booking throughout its lifecycle
/// </summary>
public enum BookingStatus
{
    /// <summary>
    /// Booking has been created but not yet confirmed
    /// </summary>
    Pending = 0,
    
    /// <summary>
    /// Booking has been confirmed and is active
    /// </summary>
    Confirmed = 1,
    
    /// <summary>
    /// Booking has been cancelled by user or admin
    /// </summary>
    Cancelled = 2,
    
    /// <summary>
    /// Booking time has passed and is completed
    /// </summary>
    Completed = 3
}
