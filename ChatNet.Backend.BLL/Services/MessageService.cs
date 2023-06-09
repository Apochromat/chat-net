using ChatNet.Backend.DAL;
using ChatNet.Backend.DAL.Entities;
using ChatNet.Common.DataTransferObjects;
using ChatNet.Common.Enumerations;
using ChatNet.Common.Exceptions;
using ChatNet.Common.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace ChatNet.Backend.BLL.Services; 

public class MessageService: IMessageService {
    private readonly BackendDbContext _dbContext;
    private readonly INotificationQueueService _notificationQueueService;

    public MessageService(BackendDbContext dbContext, INotificationQueueService notificationQueueService) {
        _dbContext = dbContext;
        _notificationQueueService = notificationQueueService;
    }

//TODO : Mentioned 
    public async Task SendMessage(MessageActionsDto message, Guid senderId, Guid chatId) {
        var user = await _dbContext.Users
            .Include(u=>u.ChatsNotificationPreferences)
            .FirstOrDefaultAsync(u => u.Id == senderId);
        if (user == null) throw new NotFoundException("User with this id does not found");
        var chat = await _dbContext.PrivateChats
            .Include(c=>c.Users)
            .FirstOrDefaultAsync(c => c.Id == chatId);
        if (chat == null) throw new NotFoundException("Chat with this id not found");
        if (chat.DeletedTime.HasValue)
            throw new MethodNotAllowedException("You can only read messages in deleted chat");
        await _dbContext.AddAsync(new Message {
             Chat = chat,
             User = user,
             TextMessage = message.TextMessage,
             Files = message.FileIds ?? new List<Guid>(),
         });
        await _dbContext.SaveChangesAsync();
        var tasks = chat.Users
            .Where(u=>u.Id != user.Id)
            .Select(async u =>
        {
            await _notificationQueueService.SendNotificationAsync(new NotificationMessageDto {
                Type = user.ChatsNotificationPreferences
                    .FirstOrDefault(n=>n.PreferencedChat == chat)!
                    .PreferenceType == NotificationPreferenceType.All?
                    NotificationMessageType.NewMessage: NotificationMessageType.NewMessageMuted,
                Title = "New message",
                Text = $"You have new message from {chat.ChatName}",
                ReceiverId = u.Id,
                SenderId = user.Id,
                ChatId = chat.Id,
                CreatedAt = DateTime.UtcNow
            });
        });
        await Task.WhenAll(tasks);
    }

    public async Task EditMessage(MessageActionsDto message, Guid messageId, Guid senderId) {
        var user = await _dbContext.Users
            .FirstOrDefaultAsync(u => u.Id == senderId);
        if (user == null) throw new NotFoundException("User with this id does not found");
        var messageDb = await _dbContext.Messages
            .Include(m=>m.User)
            .Include(m=>m.Chat)
            .FirstOrDefaultAsync(m => m.Id == messageId 
                                      && !m.DeletedTime.HasValue
            );
        if (messageDb == null) 
            throw new NotFoundException("Message with this id does not found");
        if (messageDb.Chat.DeletedTime.HasValue)
            throw new MethodNotAllowedException("You cannot edit message in deleted chat");
        if (messageDb.User != user)
            throw new MethodNotAllowedException("You don't have permission");
        messageDb.TextMessage = message.TextMessage;
        messageDb.Files = message.FileIds;
        messageDb.EditedTime = DateTime.UtcNow;
         await _dbContext.SaveChangesAsync();
    }

    public async Task DeleteMessage(Guid messageId, Guid senderId) {
        var user = await _dbContext.Users
            .FirstOrDefaultAsync(u => u.Id == senderId);
        if (user == null) throw new NotFoundException("User with this id does not found");
        var messageDb = await _dbContext.Messages
            .Include(m=>m.User)
            .Include(m=>m.Chat)
            .FirstOrDefaultAsync(m => m.Id == messageId 
                                      && !m.DeletedTime.HasValue
            );
        if (messageDb == null) 
            throw new NotFoundException("Message with this id does not found or already deleted");
        if (messageDb.Chat.DeletedTime.HasValue)
            throw new MethodNotAllowedException("You cannot delete message in deleted chat");
        if (messageDb.User != user)
            throw new MethodNotAllowedException("You don't have permission");
        messageDb.DeletedTime = DateTime.UtcNow;
        await _dbContext.SaveChangesAsync();
    }

    public async Task ViewMessage(List<Guid> messagesIds, Guid userId) {
        var user = await _dbContext.Users
            .FirstOrDefaultAsync(u => u.Id == userId);
        if (user == null) throw new NotFoundException("User with this id does not found");
        var messagesDb = await _dbContext.Messages
            .Include(m => m.Chat)
            .Where(m => messagesIds.Contains(m.Id)
                        && !m.DeletedTime.HasValue
            ).ToListAsync();
        if (messagesDb == null) 
            throw new NotFoundException("Messages not found found");
        if (messagesDb.Any(m =>m.ViewedBy.Contains(user.Id)))
            throw new ConflictException("You already viewed this message");
        messagesDb.ForEach(m=>m.ViewedBy.Add(user.Id));
        await _dbContext.SaveChangesAsync();
    }

    public async Task AddReaction(ReactionType reactionType, Guid messageId, Guid userId) {
        var user = await _dbContext.Users
            .FirstOrDefaultAsync(u => u.Id == userId);
        if (user == null) throw new NotFoundException("User with this id does not found");
        var messageDb = await _dbContext.Messages
            .Include(m=>m.Reactions)
            .ThenInclude(r=>r.Users)
            .Include(m=>m.Chat)
            .FirstOrDefaultAsync(m => m.Id == messageId 
                                      && !m.DeletedTime.HasValue
            );
        if (messageDb == null) 
            throw new NotFoundException("Message with this id does not found");
        if (messageDb.Chat.DeletedTime.HasValue)
            throw new MethodNotAllowedException("You cannot react to message in deleted chat");
        var reaction = messageDb.Reactions
            .FirstOrDefault(r => r.ReactionType == reactionType);
        if (reaction == null) {
            await _dbContext.AddAsync(new Reaction {
                Users = new List<UserBackend>{user},
                ReactionType = reactionType,
                ReactedMessage = messageDb
            });
        }
        else {
            if (reaction.Users.Contains(user))
                throw new ConflictException("You already reacted");
            reaction.Users.Add(user);
        }
        await _dbContext.SaveChangesAsync();
        if (user.Id != messageDb.User.Id) 
            await _notificationQueueService.SendNotificationAsync(new NotificationMessageDto {
                    Type = NotificationMessageType.NewReaction,
                    Title = "New reaction",
                    Text = $"Your message reacted in chat {messageDb.Chat.ChatName}",
                    ReceiverId = messageDb.User.Id,
                    SenderId = user.Id,
                    ChatId = messageDb.Chat.Id,
                    CreatedAt = DateTime.UtcNow 
            });
    }

    public async Task DeleteReaction(ReactionType reactionType, Guid messageId, Guid userId) {
        var user = await _dbContext.Users
            .FirstOrDefaultAsync(u => u.Id == userId);
        if (user == null) throw new NotFoundException("User with this id does not found");
        var messageDb = await _dbContext.Messages
            .Include(m=>m.Reactions)
            .ThenInclude(r=>r.Users)
            .Include(m=>m.Chat)
            .FirstOrDefaultAsync(m => m.Id == messageId 
                                      && !m.DeletedTime.HasValue
            );
        if (messageDb == null) 
            throw new NotFoundException("Message with this id does not found");
        if (messageDb.Chat.DeletedTime.HasValue)
            throw new MethodNotAllowedException("You cannot react to message in deleted chat");  
        var reaction = messageDb.Reactions
            .FirstOrDefault(r => r.ReactionType == reactionType);
        if (reaction == null)
            throw new NotFoundException("Reaction not found");
        if (!reaction.Users.Contains(user))
            throw new ConflictException("You didn't react to this message");
        if (reaction.Users.Count <= 1)
             _dbContext.Remove(reaction);
        else reaction.Users.Remove(user);
        await _dbContext.SaveChangesAsync();
    }
}