using ChatNet.Backend.DAL;
using ChatNet.Backend.DAL.Entities;
using ChatNet.Common.DataTransferObjects;
using ChatNet.Common.Enumerations;
using ChatNet.Common.Exceptions;
using ChatNet.Common.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace ChatNet.Backend.BLL.Services; 

public class ChatService: IChatService {
    private readonly BackendDbContext _dbContext;
    private readonly IFilesQueueService _filesQueueService;
    private readonly INotificationQueueService _notificationQueueService;

    public ChatService(BackendDbContext dbContext, IFilesQueueService filesQueueService, INotificationQueueService notificationQueue) {
        _dbContext = dbContext;
        _filesQueueService = filesQueueService;
        _notificationQueueService = notificationQueue;
    }

    public async Task<ChatListDto> GetChatList(Guid userId, int page, int pageSize) {
        if (page < 1) {
            throw new ArgumentException("Page must be greater than 0");
        }

        if (pageSize < 1) {
            throw new ArgumentException("Page size must be greater than 0");
        }
        var user = await _dbContext.Users
            .FirstOrDefaultAsync(u => u.Id == userId);
        if (user == null) 
            throw new NotFoundException("User with this id not found");
        var chats = await _dbContext.Chats
            .Where(c => c.Users
                .Contains(user))
            .OrderBy(c => c.Messages.Max(m=>m.CreatedTime))
            .ToListAsync();
        
        var pagesAmount = (int)Math.Ceiling((double)chats.Count / pageSize);
            
        if (page > pagesAmount) {
            throw new NotFoundException("Page not found");
        }
        return new ChatListDto {
            UserChats = new Pagination<ChatShortDto>(chats
                .Skip((page - 1) * pageSize)
                .Take(pageSize).ToList()
                .Select(c => new ChatShortDto {
                Id = c.Id,
                ChatAvatarId = c.ChatAvatarId,
                ChatName = c.ChatName,
                DeletedTime = c.DeletedTime //?? null
            }).ToList(),page, pageSize, pagesAmount)
        };
    }
    public async Task<Pagination<MessageDto>> GetMessages(Guid chatId, int page, int pageSize) {
        if (page < 1) {
            throw new ArgumentException("Page must be greater than 0");
        }

        if (pageSize < 1) {
            throw new ArgumentException("Page size must be greater than 0");
        }

        var chat = await _dbContext.Chats
            .FirstOrDefaultAsync(c => c.Id == chatId);
        if (chat == null)
            throw new NotFoundException("Chat with this id not found");

        var chatMessages = await _dbContext.Chats
            .Where(c => c.Id == chatId)
            .SelectMany(c => c.Messages
                .OrderByDescending(m => m.CreatedTime)
                .Select(m => new MessageDto {
                    Id = m.Id,
                    SenderId = m.User.Id,
                    TextMessage = m.TextMessage,
                    CreatedTime = m.CreatedTime,
                    EditedTime = m.EditedTime,
                    FileIds = m.Files,
                    MessageReactions = m.Reactions.Select(r=> new ReactionDto {
                        Id = r.Id,
                        Users = r.Users.Select(u=>u.Id).ToList(),
                        ReactionType = r.ReactionType
                    }).ToList(),
                    ViewedBy = m.ViewedBy
                })
            ).ToListAsync();
        
            var pagesAmount = (int)Math.Ceiling((double)chatMessages.Count / pageSize);
            
        if (page > pagesAmount) {
            throw new NotFoundException("Page not found");
        }

        return new Pagination<MessageDto>(chatMessages
            .Skip((page - 1) * pageSize)
            .Take(pageSize).ToList(), page, pageSize, pagesAmount);
    }
    public async Task<ChatFullDto> GetChatDetails(Guid chatId) {
        var chat = await _dbContext.Chats
            .Include(c=>c.FileIds)
            .Include(c => c.Users)
            .FirstOrDefaultAsync(c => c.Id == chatId);
        if (chat == null) throw new NotFoundException("Chat with this id not found");
        var response = new ChatFullDto {
            Id = chat.Id,
            ChatName = chat.ChatName,
            ChatAvatarId = chat.ChatAvatarId,
            CreatedTime = chat.CreatedTime,
            DeletedTime = chat.DeletedTime,
            FileIds = chat.FileIds,
            Users = chat.Users.Select(u => u.Id).ToList()
        };
        return response;
    }
    
    public async Task LeavePrivateChat(Guid userId, Guid chatId) {
        var user = await _dbContext.Users
            .FirstOrDefaultAsync(u => u.Id == userId);
        if (user == null) throw new NotFoundException("User with this id not found");
        
        var chat = await _dbContext.PrivateChats
            .FirstOrDefaultAsync(c => c.Id == chatId);
        if (chat == null) throw new NotFoundException("Chat with this id not found");
        user.Chats.Remove(chat);
        await _dbContext.SaveChangesAsync();
        var fileIds = chat.FileIds;
        if (chat.ChatAvatarId is not null) 
            fileIds.Add(chat.ChatAvatarId.Value);
        await _filesQueueService.SetViewersAsync(new FilesViewersDto {
            Files = fileIds,
            Viewers = chat.Users.Select(u => u.Id).ToList()
        });
    }
    public async Task LeaveGroupChat(Guid userId, Guid chatId) {
        var user = await _dbContext.Users
            .FirstOrDefaultAsync(u => u.Id == userId);
        if (user == null) throw new NotFoundException("User with this id not found");
        
        var chat = await _dbContext.GroupChats
            .FirstOrDefaultAsync(c => c.Id == chatId);
        if (chat == null) throw new NotFoundException("Chat with this id not found");
        if (chat.Administrators.Count <= 1 && chat.Administrators.Contains(user))
            throw new MethodNotAllowedException("User can not leave the chat while he is the only admin");
        user.Chats.Remove(chat);
        await _dbContext.SaveChangesAsync(); 
        var fileIds = chat.FileIds;
        if (chat.ChatAvatarId is not null) 
            fileIds.Add(chat.ChatAvatarId.Value);
        await _filesQueueService.SetViewersAsync(new FilesViewersDto {
            Files = fileIds,
            Viewers = chat.Users.Select(u => u.Id).ToList()
        });
        
    }

    public async Task CreatePrivateChat(ChatPrivateCreateDto chatModel , Guid creatorId) {
        var creator = await _dbContext.Users
            .Include(u=>u.Friends)
            .FirstOrDefaultAsync(u => u.Id == creatorId);
        if (creator == null) 
            throw new NotFoundException("Creator with this id not found");
        var user = await _dbContext.Users
            .FirstOrDefaultAsync(u => u.Id == chatModel.UserId);
        if (user == null) 
            throw new NotFoundException("Creator with this id not found");
        if (!creator.Friends.Contains(user))
            throw new MethodNotAllowedException("User must be your friend to  create chat");
        var chat = new PrivateChat {
            ChatAvatarId = chatModel.AvatarId,
            ChatName = chatModel.ChatName,
            Users = new List<UserBackend> {
                user, 
                creator,
            },
        };
        await _dbContext.AddAsync(chat);
        var preferences = new List<NotificationPreferences> {
            new() {
                PreferencedChat = chat,
                User = user,
                PreferenceType = NotificationPreferenceType.All
            },
            new() {
                PreferencedChat = chat,
                User = creator,
                PreferenceType = NotificationPreferenceType.All
            },
        };
        
        await _dbContext.AddRangeAsync(preferences);
        await _dbContext.SaveChangesAsync();
        await _notificationQueueService.SendNotificationAsync(new NotificationMessageDto {
            Type = NotificationMessageType.ChatCreated,
            Title = "New chat created",
            Text = $"You have new chat named {chat.ChatName}",
            ReceiverId = user.Id,
            SenderId = creator.Id,
            ChatId = chat.Id,
            CreatedAt = DateTime.UtcNow
        });
        await _filesQueueService.SetViewersAsync(new FilesViewersDto {
            Files = chatModel.AvatarId == null || !chatModel.AvatarId.HasValue
                ? new List<Guid>()
                : new List<Guid> { chatModel.AvatarId.Value },
            Viewers = chat.Users.Select(u => u.Id).ToList()
        });
    }

    public async Task CreateGroupChat(ChatCreateDto chatModel, Guid creatorId) {
        var admin = await _dbContext.Users
            .Include(u=>u.Friends)
            .FirstOrDefaultAsync(u => u.Id == creatorId);
        if (admin == null) 
            throw new NotFoundException("Creator with this id not found");
        var users = await _dbContext.Users
            .Where(u => chatModel.Users
                .Contains(u.Id))
            .ToListAsync();
        if (users == null || users.Count < 1 )
            throw new BadRequestException("Group chat can exist only with more than 1 user");
        if (users.Any(u => !admin.Friends.Contains(u)))
            throw new MethodNotAllowedException("Users must be your friends");
        var chat = new GroupChat {
            ChatAvatarId = chatModel.AvatarId,
            ChatName = chatModel.ChatName,
            Users = users
        };
        chat.Users.Add(admin);
        chat.Administrators.Add(admin);
        await _dbContext.AddAsync(chat);
        var preferences = users.Select(user =>
            new NotificationPreferences {
                PreferencedChat = chat,
                User = user,
                PreferenceType = NotificationPreferenceType.All
            });
        await _dbContext.AddRangeAsync(preferences);
        await _dbContext.SaveChangesAsync();
        var tasks = chat.Users
            .Where(u=>u.Id != admin.Id)
            .Select(async u =>
        {
            await _notificationQueueService.SendNotificationAsync(new NotificationMessageDto {
                Type = NotificationMessageType.ChatCreated,
                Title = "New chat created",
                Text = $"You have new chat named {chat.ChatName}",
                ReceiverId = u.Id,
                SenderId = admin.Id,
                ChatId = chat.Id,
                CreatedAt = DateTime.UtcNow
            });
        });
        await Task.WhenAll(tasks);
        await _filesQueueService.SetViewersAsync(new FilesViewersDto {
            Files = chatModel.AvatarId == null || !chatModel.AvatarId.HasValue
                ? new List<Guid>()
                : new List<Guid> { chatModel.AvatarId.Value },
            Viewers = users.Select(u => u.Id).ToList()
        });
    }

    public async Task AddUserToGroupChat(Guid chatId , Guid userId, Guid adminId) {
        var user = await _dbContext.Users
            .FirstOrDefaultAsync(u => u.Id == userId);
        if (user == null) 
            throw new NotFoundException("User with this id not found");
        var chat = await _dbContext.Chats
            .Include(c=>c.Users)
            .FirstOrDefaultAsync(c => 
                !c.DeletedTime.HasValue
                && c.Id == chatId);
        var admin = await _dbContext.Users
            .Include(u=>u.Friends)
            .FirstOrDefaultAsync(u => u.Id == userId);
        if (admin != null && !admin.Friends.Contains(user))
            throw new MethodNotAllowedException("User with this id is not your friend");
        if (chat == null) 
            throw new NotFoundException("Chat with this id not found");
        if (chat.Users.Contains(user))
            throw new ConflictException("This user is already in this chat");
        user.Chats.Add(chat);
        await _dbContext.AddAsync(new NotificationPreferences {
            PreferencedChat = chat,
            User = user,
            PreferenceType = NotificationPreferenceType.All
        });
        await _dbContext.SaveChangesAsync();
        
        var fileIds = chat.FileIds;
        if (chat.ChatAvatarId is not null) 
            fileIds.Add(chat.ChatAvatarId.Value);
        await _filesQueueService.SetViewersAsync(new FilesViewersDto {
            Files = fileIds,
            Viewers = chat.Users.Select(u => u.Id).ToList()
        });
    }

    public async Task DeleteUserFromGroupChat(Guid chatId , Guid userId) {
        var chat = await _dbContext.GroupChats
            .Include(c=>c.Administrators)
            .FirstOrDefaultAsync(c => 
                !c.DeletedTime.HasValue
                && c.Id == chatId);
        if (chat == null) 
            throw new NotFoundException("Chat with this id not found");
        var user = await _dbContext.Users
            .Include(u=>u.ChatsNotificationPreferences)
            .ThenInclude(p=>p.PreferencedChat)
            .FirstOrDefaultAsync(u => u.Id == userId);
        if (user == null) throw new NotFoundException("User with this id not found");
        if (!user.Chats.Contains(chat))
            throw new ConflictException("User with this id not exist in this chat");
        if (chat.Administrators.Contains(user)) 
            throw new MethodNotAllowedException("You can not kick administrator");
        user.Chats.Remove(chat);
        var userPreference = user.ChatsNotificationPreferences
            .FirstOrDefault(n => n.PreferencedChat == chat);
        if (userPreference != null)
            user.ChatsNotificationPreferences.Remove(userPreference);
        await _dbContext.SaveChangesAsync();
        
        var fileIds = chat.FileIds;
        if (chat.ChatAvatarId is not null) 
            fileIds.Add(chat.ChatAvatarId.Value);
        await _filesQueueService.SetViewersAsync(new FilesViewersDto {
            Files = fileIds,
            Viewers = chat.Users.Select(u => u.Id).ToList()
        });
    }

    public async Task EditChat(ChatEditDto model, Guid chatId) {
        var chat = await _dbContext.Chats
            .FirstOrDefaultAsync(c => 
                !c.DeletedTime.HasValue
                && c.Id == chatId);
        if (chat == null) throw new NotFoundException("Chat with this id not found");
        chat.ChatAvatarId = model.AvatarId;
        chat.ChatName = model.ChatName;
        await _dbContext.SaveChangesAsync();
        
        var fileIds = chat.FileIds;
        if (chat.ChatAvatarId is not null) 
            fileIds.Add(chat.ChatAvatarId.Value);
        await _filesQueueService.SetViewersAsync(new FilesViewersDto {
            Files = fileIds,
            Viewers = chat.Users.Select(u => u.Id).ToList()
        });
        
    }

    public async Task DeleteGroupChat(Guid chatId , Guid adminId) {
        var admin = await _dbContext.Users
            .FirstOrDefaultAsync(u => u.Id == adminId);
        if (admin == null) throw new NotFoundException("User with this id not found");
        var chat = await _dbContext.GroupChats
            .Include(c=>c.Users)
            .FirstOrDefaultAsync(c => 
                !c.DeletedTime.HasValue
                && c.Id == chatId);
        if (chat == null) throw new NotFoundException("Group chat with this id not found");
        chat.DeletedTime = DateTime.UtcNow;
        var preferences = chat.Users.Select(user =>
            new NotificationPreferences {
                PreferencedChat = chat,
                User = user,
                PreferenceType = NotificationPreferenceType.All
            });
         _dbContext.RemoveRange(preferences);
        await _dbContext.SaveChangesAsync();
    }

    public async Task MakeUserAdmin(Guid chatId, Guid adminId , Guid newAdminId) {
        var admin = await _dbContext.Users
            .FirstOrDefaultAsync(u => u.Id == adminId);
        if (admin == null) throw new NotFoundException("User with this id not found");
        var newAdmin = await _dbContext.Users
            .FirstOrDefaultAsync(u => u.Id == newAdminId);
        if (newAdmin == null) throw new NotFoundException("New admin with this id not found");
        var chat = await _dbContext.GroupChats
            .Include(c=>c.Administrators)
            .FirstOrDefaultAsync(c => 
                !c.DeletedTime.HasValue
                && c.Id == chatId);
        if (chat == null) throw new NotFoundException("Group chat with this id not found");
        if (chat.Administrators.Contains(newAdmin)) 
            throw new ConflictException("This user is already admin");
        chat.Administrators.Add(newAdmin);
        await _dbContext.SaveChangesAsync();
    }

    public async Task EditNotificationsPreference(Guid chatId, Guid userId, NotificationPreferenceType preferenceType) {
        var chat = await _dbContext.Chats
            .FirstOrDefaultAsync(c => 
                !c.DeletedTime.HasValue
                && c.Id == chatId);
        if (chat == null) 
            throw new NotFoundException("Chat with this id not found");
        var user = await _dbContext.Users
            .Include(u=>u.ChatsNotificationPreferences)
            .ThenInclude(p=>p.PreferencedChat)
            .FirstOrDefaultAsync(u => u.Id == userId);
        if (user == null) 
            throw new NotFoundException("User with this id not found");
        var preference = user.ChatsNotificationPreferences
            .FirstOrDefault(p => p.PreferencedChat == chat);
        if (preference != null) 
            preference.PreferenceType = preferenceType;
        else {
           await _dbContext.AddAsync(new NotificationPreferences {
                PreferencedChat = chat,
                User = user,
                PreferenceType = preferenceType
            });
        }
        await _dbContext.SaveChangesAsync();
    }
}