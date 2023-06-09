using Microsoft.EntityFrameworkCore;

namespace ChatNet.Call.DAL; 

/// <summary>
/// Call database context
/// </summary>
public class CallDbContext : DbContext {
    /// <summary>
    /// Calls table
    /// </summary>
    public DbSet<Entities.Call> Calls { get; set; } = null!; // Late initialization

    /// <inheritdoc />
    public CallDbContext(DbContextOptions<CallDbContext> options) : base(options) {
    }
}