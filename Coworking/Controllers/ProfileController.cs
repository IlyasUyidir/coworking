using Coworking.Data;
using Coworking.Models;
using Coworking.ViewModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;

namespace Coworking.Controllers;

/// <summary>
/// Controller for user profile management
/// </summary>
[Authorize]
public class ProfileController : Controller
{
    private readonly UserManager<ApplicationUser> _userManager;
    private readonly ApplicationDbContext _context;
    private readonly ILogger<ProfileController> _logger;

    public ProfileController(
        UserManager<ApplicationUser> userManager,
        ApplicationDbContext context,
        ILogger<ProfileController> logger)
    {
        _userManager = userManager;
        _context = context;
        _logger = logger;
    }

    /// <summary>
    /// View profile
    /// </summary>
    [HttpGet]
    public async Task<IActionResult> Index()
    {
        var userId = User.FindFirstValue(ClaimTypes.NameIdentifier)!;
        var user = await _userManager.FindByIdAsync(userId);
        
        if (user == null)
        {
            return NotFound();
        }

        var totalBookings = await _context.Bookings
            .CountAsync(b => b.UserId == userId);
        
        var totalSpent = await _context.Bookings
            .Where(b => b.UserId == userId && 
                       (b.Status == BookingStatus.Confirmed || b.Status == BookingStatus.Completed))
            .SumAsync(b => (decimal?)b.TotalPrice) ?? 0;

        var model = new ProfileViewModel
        {
            FullName = user.FullName,
            Email = user.Email!,
            PhoneNumber = user.PhoneNumber,
            CompanyName = user.CompanyName,
            TotalBookings = totalBookings,
            TotalSpent = totalSpent,
            MemberSince = user.LockoutEnd?.DateTime ?? DateTime.Now // Using a placeholder, ideally track registration date
        };

        return View(model);
    }

    /// <summary>
    /// Edit profile form
    /// </summary>
    [HttpGet]
    public async Task<IActionResult> Edit()
    {
        var userId = User.FindFirstValue(ClaimTypes.NameIdentifier)!;
        var user = await _userManager.FindByIdAsync(userId);
        
        if (user == null)
        {
            return NotFound();
        }

        var model = new ProfileViewModel
        {
            FullName = user.FullName,
            Email = user.Email!,
            PhoneNumber = user.PhoneNumber,
            CompanyName = user.CompanyName
        };

        return View(model);
    }

    /// <summary>
    /// Edit profile
    /// </summary>
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Edit(ProfileViewModel model)
    {
        if (ModelState.IsValid)
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier)!;
            var user = await _userManager.FindByIdAsync(userId);
            
            if (user == null)
            {
                return NotFound();
            }

            user.FullName = model.FullName;
            user.PhoneNumber = model.PhoneNumber;
            user.CompanyName = model.CompanyName;

            var result = await _userManager.UpdateAsync(user);
            
            if (result.Succeeded)
            {
                _logger.LogInformation("User profile updated: {UserId}", userId);
                TempData["Success"] = "Profile updated successfully!";
                return RedirectToAction(nameof(Index));
            }

            foreach (var error in result.Errors)
            {
                ModelState.AddModelError(string.Empty, error.Description);
            }
        }

        return View(model);
    }
}
