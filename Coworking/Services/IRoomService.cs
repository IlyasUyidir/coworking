using Coworking.Models;

namespace Coworking.Services;

/// <summary>
/// Interface for room-related operations
/// </summary>
public interface IRoomService
{
    /// <summary>
    /// Get all operational rooms
    /// </summary>
    Task<List<Room>> GetAllRoomsAsync(bool includeNonOperational = false);
    
    /// <summary>
    /// Get a room by ID with amenities
    /// </summary>
    Task<Room?> GetRoomByIdAsync(int id);
    
    /// <summary>
    /// Create a new room
    /// </summary>
    Task<Room> CreateRoomAsync(Room room, List<int> amenityIds);
    
    /// <summary>
    /// Update an existing room
    /// </summary>
    Task<bool> UpdateRoomAsync(Room room, List<int> amenityIds);
    
    /// <summary>
    /// Delete a room
    /// </summary>
    Task<bool> DeleteRoomAsync(int id);
    
    /// <summary>
    /// Get rooms filtered by capacity
    /// </summary>
    Task<List<Room>> GetRoomsByCapacityAsync(int minCapacity);
    
    /// <summary>
    /// Get all amenities
    /// </summary>
    Task<List<Amenity>> GetAllAmenitiesAsync();
    
    /// <summary>
    /// Get amenities for a specific room
    /// </summary>
    Task<List<Amenity>> GetRoomAmenitiesAsync(int roomId);
}
