using ChatNet.Notification.DAL;
using Microsoft.AspNetCore.Builder;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;

namespace ChatNet.Notification.BLL.Extensions; 

/// <summary>
/// Extension for closing opened connections
/// </summary>
public static class CloseOpenedConnectionsExtension {
    /// <summary>
    /// Migrate database
    /// </summary>
    /// <param name="app"></param>
    /// <exception cref="ArgumentNullException"></exception>
    public static async Task CloseOpenedConnectionsAsync(this WebApplication app) {
        using var serviceScope = app.Services.CreateScope();

        var context = serviceScope.ServiceProvider.GetService<NotificationDbContext>();
        if (context == null) {
            throw new ArgumentNullException(nameof(context));
        }

        var openedConnections = await context.Connections
            .Where(connection => connection.DisconnectedAt == null)
            .ToListAsync();
        context.Connections.RemoveRange(openedConnections);

        await context.SaveChangesAsync();
    }
}