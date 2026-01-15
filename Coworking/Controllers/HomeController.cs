using System.Diagnostics;
using Microsoft.AspNetCore.Mvc;
using Coworking.Models;
using Coworking.Services; // Added

namespace Coworking.Controllers;

public class HomeController : Controller
{
    private readonly ILogger<HomeController> _logger; // Added
    private readonly IRoomService _roomService; // Added

    public HomeController(ILogger<HomeController> logger, IRoomService roomService) // Modified constructor
    {
        _logger = logger;
        _roomService = roomService;
    }

    public async Task<IActionResult> Index() // Modified Index action
    {
        var featuredRooms = await _roomService.GetAllRoomsAsync();
        return View(featuredRooms);
    }

    // Removed public IActionResult Privacy()

    [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
    public IActionResult Error()
    {
        return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
    }
}