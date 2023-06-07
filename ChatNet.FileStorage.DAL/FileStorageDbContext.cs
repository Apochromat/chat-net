using ChatNet.FileStorage.DAL.Entities;
using Microsoft.EntityFrameworkCore;

namespace ChatNet.FileStorage.DAL; 

/// <summary>
/// Auth database context
/// </summary>
public class FileStorageDbContext : DbContext {
    
    /// <summary>
    /// Files table
    /// </summary>
    public DbSet<StoredFile> Files { get; set; } = null!; // Late initialization

    /// <inheritdoc />
    public FileStorageDbContext(DbContextOptions<FileStorageDbContext> options) : base(options) {
    }
}