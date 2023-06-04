using ChatNet.Common.DataTransferObjects;

namespace ChatNet.Common.Interfaces; 

public interface IMessageService {
    public Task SendMessage(MessageActionsDto message, Guid senderId, Guid chatId);
    public Task EditMessage(MessageActionsDto message,Guid messageId, Guid senderId);
    public Task DeleteMessage(Guid messageId, Guid senderId);
}