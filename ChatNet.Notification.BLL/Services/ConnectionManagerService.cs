using ChatNet.Common.Interfaces;
using ChatNet.Notification.DAL;
using ChatNet.Notification.DAL.Entities;
using Microsoft.EntityFrameworkCore;

namespace ChatNet.Notification.BLL.Services; 

/// <inheritdoc cref="IConnectionManagerService"/>
public class ConnectionManagerService : IConnectionManagerService{
    private readonly NotificationDbContext _dbContext;

    /// <summary>
    /// Constructor
    /// </summary>
    /// <param name="dbContext"></param>
    public ConnectionManagerService(NotificationDbContext dbContext) {
        _dbContext = dbContext;
    }

    /// <inheritdoc cref="IConnectionManagerService.ConnectAsync"/>
    public async Task ConnectAsync(Guid userId, string connectionId) {
        await _dbContext.Connections.AddAsync(new Connection() {
            Id = Guid.NewGuid(),
            UserId = userId,
            SignalConnectionId = connectionId
        });
        
        await _dbContext.SaveChangesAsync();
    }

    /// <inheritdoc cref="IConnectionManagerService.DisconnectAsync"/>
    public async Task DisconnectAsync(string connectionId) {
        var connection = await _dbContext.Connections.FirstOrDefaultAsync(c => 
            c.SignalConnectionId == connectionId
            && c.DisconnectedAt == null);
        if (connection == null) {
            return;
        }

        connection.DisconnectedAt = DateTime.UtcNow;
        
        _dbContext.Connections.Update(connection);
        await _dbContext.SaveChangesAsync();
    }

    /// <inheritdoc cref="IConnectionManagerService.IsConnectedAsync"/>
    public async Task<bool> IsConnectedAsync(Guid userId) {
        var connections = await _dbContext.Connections.CountAsync(c => 
            c.UserId == userId 
            && c.DisconnectedAt == null);
        return connections > 0;
    }
}