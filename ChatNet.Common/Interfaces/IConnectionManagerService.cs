namespace ChatNet.Common.Interfaces; 

/// <summary>
/// Service that manages connections to SignalR hub.
/// </summary>
public interface IConnectionManagerService {
    /// <summary>
    /// Starts connection tracking
    /// </summary>
    /// <param name="userId"></param>
    /// <param name="connectionId"></param>
    /// <returns></returns>
    Task ConnectAsync(Guid userId, string connectionId);

    /// <summary>
    /// Ends connection tracking
    /// </summary>
    /// <param name="connectionId"></param>
    /// <returns></returns>
    Task DisconnectAsync(string connectionId);

    /// <summary>
    /// Checks if user is connected
    /// </summary>
    /// <param name="userId"></param>
    /// <returns></returns>
    Task<bool> IsConnectedAsync(Guid userId);
}