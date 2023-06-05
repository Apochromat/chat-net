using ChatNet.Common.DataTransferObjects;

namespace ChatNet.Common.Interfaces; 

/// <summary>
/// Notification service to send messages to users
/// </summary>
public interface INotificationService {
    /// <summary>
    /// Send notification to user
    /// </summary>
    /// <param name="notificationMessageDto"></param>
    /// <returns></returns>
    Task<bool> SendNotificationAsync(NotificationMessageDto notificationMessageDto);
}