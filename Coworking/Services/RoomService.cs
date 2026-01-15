using Coworking.Data;
using Coworking.Models;
using Microsoft.EntityFrameworkCore;

namespace Coworking.Services;

/// <summary>
/// Implementation of room service
/// </summary>
public class RoomService : IRoomService
{
    private readonly ApplicationDbContext _context;
    private readonly ILogger<RoomService> _logger;

    public RoomService(ApplicationDbContext context, ILogger<RoomService> logger)
    {
        _context = context;
        _logger = logger;
    }

    public async Task<List<Room>> GetAllRoomsAsync(bool includeNonOperational = false)
    {
        var query = _context.Rooms
            .Include(r => r.RoomAmenities)
            .ThenInclude(ra => ra.Amenity)
            .AsQueryable();

        if (!includeNonOperational)
        {
            query = query.Where(r => r.IsOperational);
        }

        return await query.OrderBy(r => r.Name).ToListAsync();
    }

    public async Task<Room?> GetRoomByIdAsync(int id)
    {
        return await _context.Rooms
            .Include(r => r.RoomAmenities)
            .ThenInclude(ra => ra.Amenity)
            .Include(r => r.Bookings)
            .FirstOrDefaultAsync(r => r.Id == id);
    }

    public async Task<Room> CreateRoomAsync(Room room, List<int> amenityIds)
    {
        _context.Rooms.Add(room);
        await _context.SaveChangesAsync();

        // Add amenities
        if (amenityIds.Any())
        {
            var roomAmenities = amenityIds.Select(amenityId => new RoomAmenity
            {
                RoomId = room.Id,
                AmenityId = amenityId
            }).ToList();

            _context.RoomAmenities.AddRange(roomAmenities);
            await _context.SaveChangesAsync();
        }

        _logger.LogInformation("Room created: ID {RoomId}, Name {RoomName}", room.Id, room.Name);
        return room;
    }

    public async Task<bool> UpdateRoomAsync(Room room, List<int> amenityIds)
    {
        var existingRoom = await _context.Rooms.FindAsync(room.Id);
        if (existingRoom == null)
        {
            return false;
        }

        // Update room properties
        existingRoom.Name = room.Name;
        existingRoom.Capacity = room.Capacity;
        existingRoom.PricePerHour = room.PricePerHour;
        existingRoom.Description = room.Description;
        existingRoom.ImageUrl = room.ImageUrl;
        existingRoom.IsOperational = room.IsOperational;

        // Update amenities
        var existingAmenities = await _context.RoomAmenities
            .Where(ra => ra.RoomId == room.Id)
            .ToListAsync();

        _context.RoomAmenities.RemoveRange(existingAmenities);

        if (amenityIds.Any())
        {
            var newRoomAmenities = amenityIds.Select(amenityId => new RoomAmenity
            {
                RoomId = room.Id,
                AmenityId = amenityId
            }).ToList();

            _context.RoomAmenities.AddRange(newRoomAmenities);
        }

        await _context.SaveChangesAsync();
        _logger.LogInformation("Room updated: ID {RoomId}, Name {RoomName}", room.Id, room.Name);
        return true;
    }

    public async Task<bool> DeleteRoomAsync(int id)
    {
        var room = await _context.Rooms.FindAsync(id);
        if (room == null)
        {
            return false;
        }

        // Check if room has active bookings
        var hasActiveBookings = await _context.Bookings
            .AnyAsync(b => b.RoomId == id && 
                          b.Status != BookingStatus.Cancelled && 
                          b.EndTime > DateTime.UtcNow);

        if (hasActiveBookings)
        {
            throw new InvalidOperationException("Cannot delete room with active bookings.");
        }

        _context.Rooms.Remove(room);
        await _context.SaveChangesAsync();

        _logger.LogInformation("Room deleted: ID {RoomId}", id);
        return true;
    }

    public async Task<List<Room>> GetRoomsByCapacityAsync(int minCapacity)
    {
        return await _context.Rooms
            .Include(r => r.RoomAmenities)
            .ThenInclude(ra => ra.Amenity)
            .Where(r => r.IsOperational && r.Capacity >= minCapacity)
            .OrderBy(r => r.Capacity)
            .ToListAsync();
    }

    public async Task<List<Amenity>> GetAllAmenitiesAsync()
    {
        return await _context.Amenities
            .OrderBy(a => a.Name)
            .ToListAsync();
    }

    public async Task<List<Amenity>> GetRoomAmenitiesAsync(int roomId)
    {
        return await _context.RoomAmenities
            .Where(ra => ra.RoomId == roomId)
            .Select(ra => ra.Amenity)
            .ToListAsync();
    }
}
