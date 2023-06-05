using Microsoft.AspNetCore.SignalR;

namespace ChatNet.Notification.API.Services; 

/// <summary>
/// Provides the User ID for SignalR
/// </summary>
public class SignalRUserIdProvider : IUserIdProvider {
    /// <summary>
    /// Returns the User ID for SignalR
    /// </summary>
    /// <param name="connection"></param>
    /// <returns></returns>
    public string GetUserId(HubConnectionContext connection) {
        return connection.User.Identity!.Name!;
    }
}