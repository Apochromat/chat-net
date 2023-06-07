using System.ComponentModel.DataAnnotations;
using ChatNet.Common.Enumerations;

namespace ChatNet.Common.DataTransferObjects; 

/// <summary>
/// DTO for notification message to be sent to the client
/// </summary>
public class NotificationMessageDto {
    /// <summary>
    /// Notification message type
    /// </summary>
    [Required]
    public required NotificationMessageType Type { get; set; }

    /// <summary>
    /// Message title
    /// </summary>
    [Required]
    public required string Title { get; set; }

    /// <summary>
    /// Message text
    /// </summary>
    [Required]
    public required string Text { get; set; }

    /// <summary>
    /// Receiver identifier
    /// </summary>
    [Required]
    public required Guid ReceiverId { get; set; }

    /// <summary>
    /// Sender identifier
    /// </summary>
    [Required]
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
    public DateTime CreatedAt { get; set; }
}