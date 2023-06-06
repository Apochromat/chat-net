using ChatNet.Common.DataTransferObjects;

namespace ChatNet.Common.Interfaces; 

/// <summary>
/// Message service
/// </summary>
public interface IMessageService {
    /// <summary>
    /// Send message
    /// </summary>
    /// <param name="message"></param>
    /// <param name="senderId"></param>
    /// <param name="chatId"></param>
    /// <returns></returns>
    public Task SendMessage(MessageActionsDto message, Guid senderId, Guid chatId);
    /// <summary>
    /// Edit message
    /// </summary>
    /// <param name="message"></param>
    /// <param name="messageId"></param>
    /// <param name="senderId"></param>
    /// <returns></returns>
    public Task EditMessage(MessageActionsDto message,Guid messageId, Guid senderId);
    /// <summary>
    /// Delete message
    /// </summary>
    /// <param name="messageId"></param>
    /// <param name="senderId"></param>
    /// <returns></returns>
    public Task DeleteMessage(Guid messageId, Guid senderId);
}