using System.Text.Json;
using ChatNet.Common.DataTransferObjects;
using ChatNet.Common.Interfaces;
using ChatNet.Notification.API.Hubs;
using Microsoft.AspNetCore.SignalR;

namespace ChatNet.Notification.API.Services; 

/// <inheritdoc />
public class NotificationService : INotificationService {
    private readonly IHubContext<NotificationHub> _hubContext;
    private readonly IConnectionManagerService _connectionManager;

    /// <summary>
    /// Constructor
    /// </summary>
    /// <param name="hubContext"></param>
    /// <param name="connectionManager"></param>
    public NotificationService(IHubContext<NotificationHub> hubContext, IConnectionManagerService connectionManager) {
        _hubContext = hubContext;
        _connectionManager = connectionManager;
    }

    /// <inheritdoc />
    public async Task<bool> SendNotificationAsync(NotificationMessageDto notificationMessageDto) {
        if (await _connectionManager.IsConnectedAsync(notificationMessageDto.ReceiverId)) {
            await _hubContext.Clients.Group(notificationMessageDto.ReceiverId.ToString())
                .SendAsync("ReceiveMessage", JsonSerializer.Serialize(notificationMessageDto));
            return true;
        }

        return false;
    }
}