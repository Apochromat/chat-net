using ChatNet.Backend.DAL;
using ChatNet.Backend.DAL.Entities;
using ChatNet.Common.DataTransferObjects;
using ChatNet.Common.Enumerations;
using ChatNet.Common.Exceptions;
using ChatNet.Common.Interfaces;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;

namespace ChatNet.Backend.BLL.Services; 

public class MessageService: IMessageService {
    private readonly BackendDbContext _dbContext;
    private readonly INotificationQueueService _notificationQueueService;
    private readonly IFilesQueueService _filesQueueService;


    public MessageService(BackendDbContext dbContext, INotificationQueueService notificationQueueService, IFilesQueueService filesQueueService) {
        _dbContext = dbContext;
        _notificationQueueService = notificationQueueService;
        _filesQueueService = filesQueueService;
    }

    public async Task<MessageDto> GetMessageById(Guid messageId, Guid userId) {
        var user = await _dbContext.Users
            .FirstOrDefaultAsync(u => u.Id == userId);
        if (user == null) throw new NotFoundException("User with this id does not found");
        var message = await _dbContext.Messages
            .Include(m=>m.Reactions)
            .Include(m=>m.User)
            .Include(m=>m.Chat)
            .ThenInclude(c=>c.Users)
            .FirstOrDefaultAsync(m => m.Id == messageId 
                                      && !m.DeletedTime.HasValue
            );
        if (message == null) 
            throw new NotFoundException("Message with this id does not found");
        if (!message.Chat.Users.Contains(user))
            throw new ForbiddenException("You don't have permission"); 
        
        return new MessageDto {
            Id = message.Id,
            SenderId = user.Id,
            TextMessage = message.TextMessage,
            CreatedTime = message.CreatedTime,
            EditedTime = message.EditedTime,
            FileIds = message.Files,
            MessageReactions = message.Reactions.Select(r => new ReactionDto {
                Id = r.Id,
                Users = r.Users.Select(u => u.Id).ToList(),
                ReactionType = r.ReactionType
            }).ToList(),
            IsViewed = message.ViewedBy.Contains(user.Id) || message.User == user
        };
    }

    public async Task SendMessage(MessageActionsDto message, Guid senderId, Guid chatId) {
        if (message.TextMessage.IsNullOrEmpty() && message.FileIds.IsNullOrEmpty())
            throw new BadRequestException("Text or files must be in message");
        var user = await _dbContext.Users
            .Include(u=>u.ChatsNotificationPreferences)
            .FirstOrDefaultAsync(u => u.Id == senderId);
        if (user == null) throw new NotFoundException("User with this id does not found");
        var chat = await _dbContext.Chats
            .Include(c=>c.Users)
            .ThenInclude(u=>u.ChatsNotificationPreferences)
            .ThenInclude(n=>n.PreferencedChat)
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
        if (!message.FileIds.IsNullOrEmpty())
            chat.FileIds.AddRange(message.FileIds!);
        await _dbContext.SaveChangesAsync();
        var tasks = chat.Users
            .Where(u=>u.Id != user.Id)
            .Select(async u =>
        {
            await _notificationQueueService.SendNotificationAsync(new NotificationMessageDto {
                Type = u.ChatsNotificationPreferences
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
        await _filesQueueService.SetViewersAsync(new FilesViewersDto {
            Files = chat.FileIds,
            Viewers = chat.Users
                .Select(u => u.Id)
                .ToList()
        });
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

    public async Task ViewMessage(Guid messagesId, Guid userId) {
        var user = await _dbContext.Users
            .FirstOrDefaultAsync(u => u.Id == userId);
        if (user == null) throw new NotFoundException("User with this id does not found");
        var message = await _dbContext.Messages
            .Include(m=>m.User)
            .Include(m => m.Chat)
            .FirstOrDefaultAsync(m => m.Id == messagesId);
        if (message == null) 
            throw new NotFoundException("Messages not found");
        var previousMessages = await _dbContext.Messages
            .Where(m => m.Chat == message.Chat
                        && m.CreatedTime <= message.CreatedTime
                        && m.User != user
                        && !m.ViewedBy.Contains(user.Id))
            .ToListAsync();
        if (previousMessages.Any(m =>m.ViewedBy.Contains(user.Id)))
            throw new ConflictException("You already viewed this message");
        previousMessages.ForEach(m=>m.ViewedBy.Add(user.Id));
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