using System.Text.Json;
using ChatNet.Common.DataTransferObjects;
using ChatNet.Common.Enumerations;
using ChatNet.Common.Interfaces;
using Microsoft.AspNetCore.SignalR;

namespace ChatNet.Notification.API.Hubs;

/// <summary>
/// Notification hub
/// </summary>
public class NotificationHub : Hub {
    private readonly ILogger<NotificationHub> _logger;
    private readonly IConnectionManagerService _connectionManager;
    private readonly IOnlinePreferencesManagerService _onlinePreferencesManager;

    /// <summary>
    /// Constructor
    /// </summary>
    /// <param name="logger"></param>
    /// <param name="connectionManager"></param>
    /// <param name="onlinePreferencesManager"></param>
    public NotificationHub(ILogger<NotificationHub> logger, IConnectionManagerService connectionManager,
        IOnlinePreferencesManagerService onlinePreferencesManager) {
        _logger = logger;
        _connectionManager = connectionManager;
        _onlinePreferencesManager = onlinePreferencesManager;
    }

    /// <summary>
    /// User connection event
    /// </summary>
    public override async Task OnConnectedAsync() {
        if (Context.UserIdentifier == null) {
            _logger.LogError("User identifier is null");
            return;
        }

        await _connectionManager.ConnectAsync(new Guid(Context.UserIdentifier), Context.ConnectionId);
        await Groups.AddToGroupAsync(Context.ConnectionId, Context.UserIdentifier);
        _logger.LogInformation("User {Identifier} connected", Context.UserIdentifier);

        var onlinePreferences = await _onlinePreferencesManager.GetPreferenceAsync(new Guid(Context.UserIdentifier));
        switch (onlinePreferences) {
            case { Type: OnlinePreferenceType.Everyone }: {
                var notification = new NotificationMessageDto() {
                    ReceiverId = Guid.Empty,
                    SenderId = Guid.Parse(Context.UserIdentifier),
                    Type = NotificationMessageType.UserOnline,
                    Title = "User online",
                    Text = "User became online",
                    CreatedAt = DateTime.UtcNow
                };

                await Clients.All.SendAsync("ReceiveMessage", JsonSerializer.Serialize(notification));
                break;
            }
            case { Type: OnlinePreferenceType.Friends }: {
                foreach (var friend in onlinePreferences.Friends) {
                    var notification = new NotificationMessageDto() {
                        ReceiverId = friend,
                        SenderId = Guid.Parse(Context.UserIdentifier),
                        Type = NotificationMessageType.UserOnline,
                        Title = "User online",
                        Text = "User became online",
                        CreatedAt = DateTime.UtcNow
                    };

                    await Clients.User(friend.ToString())
                        .SendAsync("ReceiveMessage", JsonSerializer.Serialize(notification));
                }

                break;
            }
        }

        await base.OnConnectedAsync();
    }

    /// <summary>
    /// User disconnection event
    /// </summary>
    /// <param name="exception"></param>
    public override async Task OnDisconnectedAsync(Exception? exception) {
        if (Context.UserIdentifier == null) {
            _logger.LogError("User identifier is null");
            return;
        }

        await _connectionManager.DisconnectAsync(Context.ConnectionId);
        await Groups.RemoveFromGroupAsync(Context.ConnectionId, Context.UserIdentifier);
        _logger.LogInformation("User {Identifier} disconnected", Context.UserIdentifier);

        var onlinePreferences = await _onlinePreferencesManager.GetPreferenceAsync(new Guid(Context.UserIdentifier));
        switch (onlinePreferences) {
            case { Type: OnlinePreferenceType.Everyone }: {
                var notification = new NotificationMessageDto() {
                    ReceiverId = Guid.Empty,
                    SenderId = Guid.Parse(Context.UserIdentifier),
                    Type = NotificationMessageType.UserOffline,
                    Title = "User offline",
                    Text = "User became offline",
                    CreatedAt = DateTime.UtcNow
                };

                await Clients.All.SendAsync("ReceiveMessage", JsonSerializer.Serialize(notification));
                break;
            }
            case { Type: OnlinePreferenceType.Friends }: {
                foreach (var friend in onlinePreferences.Friends) {
                    var notification = new NotificationMessageDto() {
                        ReceiverId = friend,
                        SenderId = Guid.Parse(Context.UserIdentifier),
                        Type = NotificationMessageType.UserOffline,
                        Title = "User offline",
                        Text = "User became offline",
                        CreatedAt = DateTime.UtcNow
                    };

                    await Clients.User(friend.ToString())
                        .SendAsync("ReceiveMessage", JsonSerializer.Serialize(notification));
                }

                break;
            }
        }

        await base.OnDisconnectedAsync(exception);
    }
}