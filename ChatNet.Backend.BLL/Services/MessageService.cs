using ChatNet.Backend.DAL;
using ChatNet.Backend.DAL.Entities;
using ChatNet.Common.DataTransferObjects;
using ChatNet.Common.Exceptions;
using ChatNet.Common.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace ChatNet.Backend.BLL.Services; 

public class MessageService: IMessageService {
    private readonly BackendDbContext _dbContext;
    public MessageService(BackendDbContext dbContext) {
        _dbContext = dbContext;
    }


    public async Task SendMessage(MessageActionsDto message, Guid senderId, Guid chatId) {
        var user = await _dbContext.Users
            .FirstOrDefaultAsync(u => u.Id == senderId);
        if (user == null) throw new NotFoundException("user with this id does not found");
        var chat = await _dbContext.PrivateChats
            .FirstOrDefaultAsync(c => c.Id == chatId);
        if (chat == null) throw new NotFoundException("Chat with this id not found");
        if (chat.DeletedTime.HasValue)
            throw new MethodNotAllowedException("you can only read messages in deleted chat");
        await _dbContext.AddAsync(new Message {
             Chat = chat,
             User = user,
             TextMessage = message.TextMessage,
             Files = message.FileIds,
         });
        await _dbContext.SaveChangesAsync();
            //TODO _notificationService.SendMessage(userList)
    }

    public async Task EditMessage(MessageActionsDto message, Guid messageId, Guid senderId) {
        var user = await _dbContext.Users
            .FirstOrDefaultAsync(u => u.Id == senderId);
        if (user == null) throw new NotFoundException("user with this id does not found");
        var messageDb = await _dbContext.Messages
            .Include(m=>m.Chat)
            .FirstOrDefaultAsync(m => m.Id == senderId 
                                      && !m.DeletedTime.HasValue
            );
        if (messageDb == null) 
            throw new NotFoundException("message with this id does not found");
        if (messageDb.Chat.DeletedTime.HasValue)
            throw new MethodNotAllowedException("you cannot edit message in deleted chat");
        messageDb.TextMessage = message.TextMessage;
        messageDb.Files = message.FileIds;
        messageDb.EditedTime = DateTime.UtcNow;
         await _dbContext.SaveChangesAsync();
    }

    public async Task DeleteMessage(Guid messageId, Guid senderId) {
        var user = await _dbContext.Users
            .FirstOrDefaultAsync(u => u.Id == senderId);
        if (user == null) throw new NotFoundException("user with this id does not found");
        var messageDb = await _dbContext.Messages
            .Include(m=>m.Chat)
            .FirstOrDefaultAsync(m => m.Id == senderId 
                                      && !m.DeletedTime.HasValue
            );
        if (messageDb == null) 
            throw new NotFoundException("message with this id does not found or already deleted");
        messageDb.DeletedTime = DateTime.UtcNow;
        await _dbContext.SaveChangesAsync();
    }
}