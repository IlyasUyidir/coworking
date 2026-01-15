namespace Coworking.Utilities;

/// <summary>
/// Application-wide constants
/// </summary>
public static class Constants
{
    /// <summary>
    /// User role constants
    /// </summary>
    public static class Roles
    {
        public const string Administrator = "Administrator";
        public const string Client = "Client";
    }
    
    /// <summary>
    /// Booking status display names
    /// </summary>
    public static class BookingStatuses
    {
        public const string Pending = "Pending";
        public const string Confirmed = "Confirmed";
        public const string Cancelled = "Cancelled";
        public const string Completed = "Completed";
    }
    
    /// <summary>
    /// Default values
    /// </summary>
    public static class Defaults
    {
        public const string DefaultRoomImage = "/images/rooms/default-room.jpg";
        public const int MinBookingDurationHours = 1;
        public const int MaxBookingDurationHours = 24;
    }
}
