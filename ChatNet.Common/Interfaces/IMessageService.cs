using ChatNet.Common.DataTransferObjects;
using ChatNet.Common.Enumerations;

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

    /// <summary>
    /// View message 
    /// </summary>
    /// <param name="messageId"></param>
    /// <param name="userId"></param>
    /// <returns></returns>
    public Task ViewMessage(Guid messageId, Guid userId);

    /// <summary>
    /// Add reaction
    /// </summary>
    /// <param name="reactionType"></param>
    /// <param name="messageId"></param>
    /// <param name="userId"></param>
    /// <returns></returns>
    public Task AddReaction(ReactionType reactionType, Guid messageId, Guid userId);
    /// <summary>
    /// Delete reaction
    /// </summary>
    /// <param name="reactionType"></param>
    /// <param name="messageId"></param>
    /// <param name="userId"></param>
    /// <returns></returns>
    public Task DeleteReaction(ReactionType reactionType, Guid messageId, Guid userId);
}