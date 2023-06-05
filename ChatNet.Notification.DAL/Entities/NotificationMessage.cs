using ChatNet.Common.Enumerations;

namespace ChatNet.Notification.DAL.Entities; 

/// <summary>
/// Notification message entity.
/// </summary>
public class NotificationMessage {
    /// <summary>
    /// Notification message identifier
    /// </summary>
    public Guid Id { get; set; }
    
    /// <summary>
    /// Notification message type
    /// </summary>
    public required NotificationMessageType Type { get; set; }

    /// <summary>
    /// Message title
    /// </summary>
    public required string Title { get; set; }

    /// <summary>
    /// Message text
    /// </summary>
    public required string Text { get; set; }

    /// <summary>
    /// Receiver identifier
    /// </summary>
    public required Guid ReceiverId { get; set; }

    /// <summary>
    /// Sender identifier
    /// </summary>
    public required Guid SenderId { get; set; }
    
    /// <summary>
    /// Chat identifier
    /// </summary>
    public Guid? ChatId { get; set; }
    
    /// <summary>
    /// Call identifier
    /// </summary>
    public Guid? CallId { get; set; }
    
    /// <summary>
    /// Message creation date
    /// </summary>
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

    /// <summary>
    /// Message delivery date
    /// </summary>
    public DateTime? DeliveredAt { get; set; }
}