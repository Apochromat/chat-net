using ChatNet.Auth.DAL.Entities;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace ChatNet.Auth.DAL; 

/// <summary>
/// Auth database context
/// </summary>
public class AuthDbContext : IdentityDbContext<User, IdentityRole<Guid>, Guid, IdentityUserClaim<Guid>,
    IdentityUserRole<Guid>, IdentityUserLogin<Guid>, IdentityRoleClaim<Guid>, IdentityUserToken<Guid>> {
    
    /// <summary>
    /// Users table
    /// </summary>
    public new DbSet<User> Users { get; set; } = null!; // Late initialization
    
    /// <summary>
    /// Devices table
    /// </summary>
    public DbSet<Device> Devices { get; set; } = null!; // Late initialization

    /// <inheritdoc />
    public AuthDbContext(DbContextOptions<AuthDbContext> options) : base(options) {
    }
}