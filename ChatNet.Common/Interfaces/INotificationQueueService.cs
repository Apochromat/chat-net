using ChatNet.Common.DataTransferObjects;

namespace ChatNet.Common.Interfaces; 

/// <summary>
/// Notification queue service
/// </summary>
public interface INotificationQueueService {
    /// <summary>
    /// Sends notification message to notification component
    /// </summary>
    /// <param name="notificationMessageDto"></param>
    /// <returns></returns>
    Task SendNotificationAsync(NotificationMessageDto notificationMessageDto);
    
    /// <summary>
    /// Sends online preference to notification component
    /// </summary>
    /// <param name="userId"></param>
    /// <param name="onlinePreferenceDto"></param>
    /// <returns></returns>
    Task SendOnlinePreferenceAsync(Guid userId, OnlinePreferenceDto onlinePreferenceDto);
}