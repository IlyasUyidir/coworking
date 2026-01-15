using Coworking.Models;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace Coworking.Data;

/// <summary>
/// Database context for the Coworking application
/// </summary>
public class ApplicationDbContext : IdentityDbContext<ApplicationUser>
{
    public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) 
        : base(options)
    {
    }
    
    /// <summary>
    /// Rooms available for booking
    /// </summary>
    public DbSet<Room> Rooms { get; set; }
    
    /// <summary>
    /// Bookings made by users
    /// </summary>
    public DbSet<Booking> Bookings { get; set; }
    
    /// <summary>
    /// Available amenities
    /// </summary>
    public DbSet<Amenity> Amenities { get; set; }
    
    /// <summary>
    /// Junction table for Room-Amenity many-to-many relationship
    /// </summary>
    public DbSet<RoomAmenity> RoomAmenities { get; set; }
    
    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);
        
        // Configure RoomAmenity composite key
        modelBuilder.Entity<RoomAmenity>()
            .HasKey(ra => new { ra.RoomId, ra.AmenityId });
        
        // Configure Room-RoomAmenity relationship
        modelBuilder.Entity<RoomAmenity>()
            .HasOne(ra => ra.Room)
            .WithMany(r => r.RoomAmenities)
            .HasForeignKey(ra => ra.RoomId)
            .OnDelete(DeleteBehavior.Cascade);
        
        // Configure Amenity-RoomAmenity relationship
        modelBuilder.Entity<RoomAmenity>()
            .HasOne(ra => ra.Amenity)
            .WithMany(a => a.RoomAmenities)
            .HasForeignKey(ra => ra.AmenityId)
            .OnDelete(DeleteBehavior.Cascade);
        
        // Configure Room-Booking relationship
        modelBuilder.Entity<Booking>()
            .HasOne(b => b.Room)
            .WithMany(r => r.Bookings)
            .HasForeignKey(b => b.RoomId)
            .OnDelete(DeleteBehavior.Cascade);
        
        // Configure ApplicationUser-Booking relationship
        modelBuilder.Entity<Booking>()
            .HasOne(b => b.User)
            .WithMany(u => u.Bookings)
            .HasForeignKey(b => b.UserId)
            .OnDelete(DeleteBehavior.Cascade);
        
        // Configure decimal precision for Room.PricePerHour
        modelBuilder.Entity<Room>()
            .Property(r => r.PricePerHour)
            .HasPrecision(18, 2);
        
        // Configure decimal precision for Booking.TotalPrice
        modelBuilder.Entity<Booking>()
            .Property(b => b.TotalPrice)
            .HasPrecision(18, 2);
        
        // Add index on Booking.StartTime and EndTime for performance
        modelBuilder.Entity<Booking>()
            .HasIndex(b => new { b.RoomId, b.StartTime, b.EndTime });
        
        // Add index on Room.IsOperational for filtering
        modelBuilder.Entity<Room>()
            .HasIndex(r => r.IsOperational);
    }
}