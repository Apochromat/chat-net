using ChatNet.Notification.DAL.Entities;
using Microsoft.EntityFrameworkCore;

namespace ChatNet.Notification.DAL;

/// <summary>
/// Notification database context
/// </summary>
public class NotificationDbContext : DbContext {
    /// <summary>
    /// Notification messages table
    /// </summary>
    public DbSet<NotificationMessage> Messages { get; set; } = null!; // Late initialization

    /// <summary>
    /// Connections table
    /// </summary>
    public DbSet<Connection> Connections { get; set; } = null!; // Late initialization
    
    /// <summary>
    /// Online preferences table
    /// </summary>
    public DbSet<OnlinePreference> OnlinePreferences { get; set; } = null!; // Late initialization

    /// <inheritdoc />
    public NotificationDbContext(DbContextOptions<NotificationDbContext> options) : base(options) {
    }
}