namespace ChatNet.Notification.DAL.Entities; 

/// <summary>
/// Connection entity
/// </summary>
public class Connection {
    /// <summary>
    /// Entity Identifier
    /// </summary>
    public Guid Id { get; set; }

    /// <summary>
    /// User identifier
    /// </summary>
    public Guid UserId { get; set; }

    /// <summary>
    /// Signalr Connection Identifier
    /// </summary>
    public required string SignalConnectionId { get; set; }

    /// <summary>
    /// Connection creation date
    /// </summary>
    public DateTime ConnectedAt { get; set; } = DateTime.UtcNow;
    
    /// <summary>
    /// Disconnection date
    /// </summary>
    public DateTime? DisconnectedAt { get; set; } = null;
}