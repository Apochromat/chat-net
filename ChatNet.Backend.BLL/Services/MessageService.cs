using ChatNet.Common.DataTransferObjects;
using ChatNet.Common.Interfaces;

namespace ChatNet.Backend.BLL.Services; 

public class MessageService: IMessageService {
    public async Task SendMessage(MessageActionsDto message, Guid senderId, Guid chatId) {
        throw new NotImplementedException();
    }

    public async Task EditMessage(MessageActionsDto message, Guid chatId, Guid senderId) {
        throw new NotImplementedException();
    }

    public async Task DeleteMessage(Guid chatId, Guid senderId) {
        throw new NotImplementedException();
    }
}