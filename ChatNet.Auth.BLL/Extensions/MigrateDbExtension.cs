using ChatNet.Auth.DAL;
using Microsoft.AspNetCore.Builder;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;

namespace ChatNet.Auth.BLL.Extensions; 

/// <summary>
/// Extension for database migration
/// </summary>
public static class MigrateDbExtension {
    /// <summary>
    /// Migrate database
    /// </summary>
    /// <param name="app"></param>
    /// <exception cref="ArgumentNullException"></exception>
    public static async Task MigrateDbAsync(this WebApplication app) {
        using var serviceScope = app.Services.CreateScope();

        // Migrate database
        var context = serviceScope.ServiceProvider.GetService<AuthDbContext>();
        if (context == null) {
            throw new ArgumentNullException(nameof(context));
        }

        await context.Database.MigrateAsync();
    }
}