using Coworking.Models;
using Coworking.Services;
using Coworking.Utilities;
using Coworking.ViewModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Coworking.Controllers;

/// <summary>
/// Controller for room management and browsing
/// </summary>
public class RoomsController : Controller
{
    private readonly IRoomService _roomService;
    private readonly ILogger<RoomsController> _logger;

    public RoomsController(IRoomService roomService, ILogger<RoomsController> logger)
    {
        _roomService = roomService;
        _logger = logger;
    }

    /// <summary>
    /// Browse available rooms (public)
    /// </summary>
    [HttpGet]
    [AllowAnonymous]
    public async Task<IActionResult> Index(int? capacity)
    {
        List<Room> rooms;
        
        if (capacity.HasValue && capacity.Value > 0)
        {
            rooms = await _roomService.GetRoomsByCapacityAsync(capacity.Value);
        }
        else
        {
            rooms = await _roomService.GetAllRoomsAsync();
        }

        ViewBag.SelectedCapacity = capacity;
        return View(rooms);
    }

    /// <summary>
    /// View room details
    /// </summary>
    [HttpGet]
    public async Task<IActionResult> Details(int id)
    {
        var room = await _roomService.GetRoomByIdAsync(id);
        
        if (room == null)
        {
            return NotFound();
        }

        return View(room);
    }

    /// <summary>
    /// Manage rooms (Admin only)
    /// </summary>
    [HttpGet]
    [Authorize(Roles = Constants.Roles.Administrator)]
    public async Task<IActionResult> Manage()
    {
        var rooms = await _roomService.GetAllRoomsAsync(includeNonOperational: true);
        return View(rooms);
    }

    /// <summary>
    /// Create new room form (Admin only)
    /// </summary>
    [HttpGet]
    [Authorize(Roles = Constants.Roles.Administrator)]
    public async Task<IActionResult> Create()
    {
        var amenities = await _roomService.GetAllAmenitiesAsync();
        var model = new RoomViewModel
        {
            AllAmenities = amenities,
            IsOperational = true
        };
        return View(model);
    }

    /// <summary>
    /// Create new room (Admin only)
    /// </summary>
    [HttpPost]
    [Authorize(Roles = Constants.Roles.Administrator)]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Create(RoomViewModel model)
    {
        if (ModelState.IsValid)
        {
            var room = new Room
            {
                Name = model.Name,
                Capacity = model.Capacity,
                PricePerHour = model.PricePerHour,
                Description = model.Description,
                ImageUrl = string.IsNullOrEmpty(model.ImageUrl) ? Constants.Defaults.DefaultRoomImage : model.ImageUrl,
                IsOperational = model.IsOperational
            };

            await _roomService.CreateRoomAsync(room, model.SelectedAmenityIds);
            _logger.LogInformation("Room created: {RoomName}", room.Name);
            
            TempData["Success"] = "Room created successfully!";
            return RedirectToAction(nameof(Manage));
        }

        model.AllAmenities = await _roomService.GetAllAmenitiesAsync();
        return View(model);
    }

    /// <summary>
    /// Edit room form (Admin only)
    /// </summary>
    [HttpGet]
    [Authorize(Roles = Constants.Roles.Administrator)]
    public async Task<IActionResult> Edit(int id)
    {
        var room = await _roomService.GetRoomByIdAsync(id);
        
        if (room == null)
        {
            return NotFound();
        }

        var amenities = await _roomService.GetAllAmenitiesAsync();
        var selectedAmenityIds = room.RoomAmenities.Select(ra => ra.AmenityId).ToList();

        var model = new RoomViewModel
        {
            Id = room.Id,
            Name = room.Name,
            Capacity = room.Capacity,
            PricePerHour = room.PricePerHour,
            Description = room.Description,
            ImageUrl = room.ImageUrl,
            IsOperational = room.IsOperational,
            AllAmenities = amenities,
            SelectedAmenityIds = selectedAmenityIds
        };

        return View(model);
    }

    /// <summary>
    /// Edit room (Admin only)
    /// </summary>
    [HttpPost]
    [Authorize(Roles = Constants.Roles.Administrator)]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Edit(int id, RoomViewModel model)
    {
        if (id != model.Id)
        {
            return NotFound();
        }

        if (ModelState.IsValid)
        {
            var room = new Room
            {
                Id = model.Id,
                Name = model.Name,
                Capacity = model.Capacity,
                PricePerHour = model.PricePerHour,
                Description = model.Description,
                ImageUrl = model.ImageUrl,
                IsOperational = model.IsOperational
            };

            var result = await _roomService.UpdateRoomAsync(room, model.SelectedAmenityIds);
            
            if (result)
            {
                _logger.LogInformation("Room updated: {RoomName}", room.Name);
                TempData["Success"] = "Room updated successfully!";
                return RedirectToAction(nameof(Manage));
            }
            
            return NotFound();
        }

        model.AllAmenities = await _roomService.GetAllAmenitiesAsync();
        return View(model);
    }

    /// <summary>
    /// Delete room confirmation (Admin only)
    /// </summary>
    [HttpGet]
    [Authorize(Roles = Constants.Roles.Administrator)]
    public async Task<IActionResult> Delete(int id)
    {
        var room = await _roomService.GetRoomByIdAsync(id);
        
        if (room == null)
        {
            return NotFound();
        }

        return View(room);
    }

    /// <summary>
    /// Delete room (Admin only)
    /// </summary>
    [HttpPost, ActionName("Delete")]
    [Authorize(Roles = Constants.Roles.Administrator)]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> DeleteConfirmed(int id)
    {
        try
        {
            var result = await _roomService.DeleteRoomAsync(id);
            
            if (result)
            {
                _logger.LogInformation("Room deleted: ID {RoomId}", id);
                TempData["Success"] = "Room deleted successfully!";
            }
            else
            {
                TempData["Error"] = "Room not found.";
            }
        }
        catch (InvalidOperationException ex)
        {
            _logger.LogWarning("Cannot delete room: {Message}", ex.Message);
            TempData["Error"] = ex.Message;
        }

        return RedirectToAction(nameof(Manage));
    }
}
