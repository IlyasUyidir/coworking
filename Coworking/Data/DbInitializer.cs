using Coworking.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

namespace Coworking.Data;

/// <summary>
/// Initializes the database with seed data
/// </summary>
public static class DbInitializer
{
    /// <summary>
    /// Initializes the database and seeds initial data
    /// </summary>
    public static async Task Initialize(IServiceProvider serviceProvider)
    {
        using var scope = serviceProvider.CreateScope();
        var context = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();
        var userManager = scope.ServiceProvider.GetRequiredService<UserManager<ApplicationUser>>();
        var roleManager = scope.ServiceProvider.GetRequiredService<RoleManager<IdentityRole>>();
        
        // Ensure database is created
        await context.Database.EnsureCreatedAsync();
        
        // Seed roles
        await SeedRoles(roleManager);
        
        // Seed admin user
        await SeedAdminUser(userManager);
        
        // Seed amenities
        await SeedAmenities(context);
        
        // Seed rooms
        await SeedRooms(context);
        
        // Seed room-amenity associations
        await SeedRoomAmenities(context);
    }
    
    private static async Task SeedRoles(RoleManager<IdentityRole> roleManager)
    {
        string[] roles = { "Administrator", "Client" };
        
        foreach (var role in roles)
        {
            if (!await roleManager.RoleExistsAsync(role))
            {
                await roleManager.CreateAsync(new IdentityRole(role));
            }
        }
    }
    
    private static async Task SeedAdminUser(UserManager<ApplicationUser> userManager)
    {
        const string adminEmail = "admin@coworking.com";
        
        if (await userManager.FindByEmailAsync(adminEmail) == null)
        {
            var adminUser = new ApplicationUser
            {
                UserName = adminEmail,
                Email = adminEmail,
                FullName = "System Administrator",
                EmailConfirmed = true,
                PhoneNumber = "+1234567890"
            };
            
            var result = await userManager.CreateAsync(adminUser, "Admin@123");
            
            if (result.Succeeded)
            {
                await userManager.AddToRoleAsync(adminUser, "Administrator");
            }
        }
    }
    
    private static async Task SeedAmenities(ApplicationDbContext context)
    {
        if (await context.Amenities.AnyAsync())
        {
            return; // Amenities already seeded
        }
        
        var amenities = new List<Amenity>
        {
            new() { Name = "WiFi", IconCssClass = "fa-wifi" },
            new() { Name = "Projector", IconCssClass = "fa-video" },
            new() { Name = "Whiteboard", IconCssClass = "fa-chalkboard" },
            new() { Name = "Coffee Machine", IconCssClass = "fa-mug-hot" },
            new() { Name = "Parking", IconCssClass = "fa-parking" },
            new() { Name = "Air Conditioning", IconCssClass = "fa-snowflake" },
            new() { Name = "Printer", IconCssClass = "fa-print" },
            new() { Name = "Phone", IconCssClass = "fa-phone" }
        };
        
        await context.Amenities.AddRangeAsync(amenities);
        await context.SaveChangesAsync();
    }
    
    private static async Task SeedRooms(ApplicationDbContext context)
    {
        if (await context.Rooms.AnyAsync())
        {
            return; // Rooms already seeded
        }
        
        var rooms = new List<Room>
        {
            new()
            {
                Name = "Executive Conference Room",
                Capacity = 12,
                PricePerHour = 50.00m,
                Description = "Spacious conference room perfect for board meetings and presentations. Features a large table, comfortable seating, and state-of-the-art presentation equipment.",
                ImageUrl = "/images/rooms/conference-room.jpg",
                IsOperational = true
            },
            new()
            {
                Name = "Private Office 1",
                Capacity = 2,
                PricePerHour = 25.00m,
                Description = "Quiet private office ideal for focused work or small team collaborations. Includes a desk, ergonomic chairs, and high-speed internet.",
                ImageUrl = "/images/rooms/private-office-1.jpg",
                IsOperational = true
            },
            new()
            {
                Name = "Creative Studio",
                Capacity = 8,
                PricePerHour = 40.00m,
                Description = "Bright and inspiring space designed for creative teams. Features flexible seating, whiteboards, and plenty of natural light.",
                ImageUrl = "/images/rooms/creative-studio.jpg",
                IsOperational = true
            },
            new()
            {
                Name = "Meeting Pod",
                Capacity = 4,
                PricePerHour = 20.00m,
                Description = "Compact meeting space perfect for quick huddles and small team discussions. Equipped with a display screen and video conferencing capabilities.",
                ImageUrl = "/images/rooms/meeting-pod.jpg",
                IsOperational = true
            },
            new()
            {
                Name = "Training Room",
                Capacity = 20,
                PricePerHour = 75.00m,
                Description = "Large training room suitable for workshops, seminars, and team training sessions. Features theater-style seating and professional AV equipment.",
                ImageUrl = "/images/rooms/training-room.jpg",
                IsOperational = true
            }
        };
        
        await context.Rooms.AddRangeAsync(rooms);
        await context.SaveChangesAsync();
    }
    
    private static async Task SeedRoomAmenities(ApplicationDbContext context)
    {
        if (await context.RoomAmenities.AnyAsync())
        {
            return; // Room amenities already seeded
        }
        
        var rooms = await context.Rooms.ToListAsync();
        var amenities = await context.Amenities.ToListAsync();
        
        var wifiId = amenities.First(a => a.Name == "WiFi").Id;
        var projectorId = amenities.First(a => a.Name == "Projector").Id;
        var whiteboardId = amenities.First(a => a.Name == "Whiteboard").Id;
        var coffeeId = amenities.First(a => a.Name == "Coffee Machine").Id;
        var parkingId = amenities.First(a => a.Name == "Parking").Id;
        var acId = amenities.First(a => a.Name == "Air Conditioning").Id;
        var printerId = amenities.First(a => a.Name == "Printer").Id;
        var phoneId = amenities.First(a => a.Name == "Phone").Id;
        
        var roomAmenities = new List<RoomAmenity>();
        
        // Executive Conference Room - all amenities
        var conferenceRoom = rooms.First(r => r.Name == "Executive Conference Room");
        roomAmenities.AddRange(new[]
        {
            new RoomAmenity { RoomId = conferenceRoom.Id, AmenityId = wifiId },
            new RoomAmenity { RoomId = conferenceRoom.Id, AmenityId = projectorId },
            new RoomAmenity { RoomId = conferenceRoom.Id, AmenityId = whiteboardId },
            new RoomAmenity { RoomId = conferenceRoom.Id, AmenityId = coffeeId },
            new RoomAmenity { RoomId = conferenceRoom.Id, AmenityId = parkingId },
            new RoomAmenity { RoomId = conferenceRoom.Id, AmenityId = acId },
            new RoomAmenity { RoomId = conferenceRoom.Id, AmenityId = phoneId }
        });
        
        // Private Office 1
        var privateOffice = rooms.First(r => r.Name == "Private Office 1");
        roomAmenities.AddRange(new[]
        {
            new RoomAmenity { RoomId = privateOffice.Id, AmenityId = wifiId },
            new RoomAmenity { RoomId = privateOffice.Id, AmenityId = acId },
            new RoomAmenity { RoomId = privateOffice.Id, AmenityId = phoneId }
        });
        
        // Creative Studio
        var creativeStudio = rooms.First(r => r.Name == "Creative Studio");
        roomAmenities.AddRange(new[]
        {
            new RoomAmenity { RoomId = creativeStudio.Id, AmenityId = wifiId },
            new RoomAmenity { RoomId = creativeStudio.Id, AmenityId = whiteboardId },
            new RoomAmenity { RoomId = creativeStudio.Id, AmenityId = coffeeId },
            new RoomAmenity { RoomId = creativeStudio.Id, AmenityId = acId },
            new RoomAmenity { RoomId = creativeStudio.Id, AmenityId = printerId }
        });
        
        // Meeting Pod
        var meetingPod = rooms.First(r => r.Name == "Meeting Pod");
        roomAmenities.AddRange(new[]
        {
            new RoomAmenity { RoomId = meetingPod.Id, AmenityId = wifiId },
            new RoomAmenity { RoomId = meetingPod.Id, AmenityId = projectorId },
            new RoomAmenity { RoomId = meetingPod.Id, AmenityId = acId }
        });
        
        // Training Room
        var trainingRoom = rooms.First(r => r.Name == "Training Room");
        roomAmenities.AddRange(new[]
        {
            new RoomAmenity { RoomId = trainingRoom.Id, AmenityId = wifiId },
            new RoomAmenity { RoomId = trainingRoom.Id, AmenityId = projectorId },
            new RoomAmenity { RoomId = trainingRoom.Id, AmenityId = whiteboardId },
            new RoomAmenity { RoomId = trainingRoom.Id, AmenityId = coffeeId },
            new RoomAmenity { RoomId = trainingRoom.Id, AmenityId = parkingId },
            new RoomAmenity { RoomId = trainingRoom.Id, AmenityId = acId },
            new RoomAmenity { RoomId = trainingRoom.Id, AmenityId = printerId }
        });
        
        await context.RoomAmenities.AddRangeAsync(roomAmenities);
        await context.SaveChangesAsync();
    }
}
