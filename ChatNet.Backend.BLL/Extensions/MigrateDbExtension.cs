using ChatNet.Backend.DAL;
using Microsoft.AspNetCore.Builder;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;

namespace ChatNet.Backend.BLL.Extensions; 

public static class MigrateDbExtension {
    public static async Task MigrateDbAsync(this WebApplication app) {
        using var serviceScope = app.Services.CreateScope();

        // Migrate database
        var context = serviceScope.ServiceProvider.GetService<BackendDbContext>();
        if (context == null) {
            throw new ArgumentNullException(nameof(context));
        }
        await context.Database.MigrateAsync();
    }
}