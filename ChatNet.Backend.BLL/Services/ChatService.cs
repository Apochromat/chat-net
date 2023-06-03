using ChatNet.Backend.DAL;
using ChatNet.Backend.DAL.Entities;
using ChatNet.Common.DataTransferObjects;
using ChatNet.Common.Exceptions;
using ChatNet.Common.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace ChatNet.Backend.BLL.Services; 

public class ChatService: IChatService {
    private readonly BackendDbContext _dbContext;

    public ChatService(BackendDbContext dbContext) {
        _dbContext = dbContext;
    }

    public async Task<ChatListDto> GetChatList(Guid userId) {
        var user = await _dbContext.Users
            .FirstOrDefaultAsync(u => u.Id == userId);
        if (user == null) throw new NotFoundException("user with this id does not found");
        var chats = await _dbContext.Chats
            .Where(c => c.Users
                .Contains(user)
            && !c.DeletedTime.HasValue)
            .ToListAsync();
        return new ChatListDto {
            UserChats = chats.Select(c => new ChatShortDto {
                Id = c.Id,
                ChatAvatarId = c.ChatAvatarId,
                ChatName = c.ChatName
            }).ToList()
        };
        return null;
    }
//TODO 
    public async Task<ChatFullDto> GetChatWithMessages(Guid chatId) {
        throw new NotImplementedException();
    }
//TODO 
    public async Task LeavePrivateChat(Guid userId, Guid chatId) {
        var user = await _dbContext.Users
            .FirstOrDefaultAsync(u => u.Id == userId);
        if (user == null) throw new NotFoundException("user with this id does not found");
        
        var chat = await _dbContext.PrivateChats
            .FirstOrDefaultAsync(c => c.Id == chatId);
        if (chat == null) throw new NotFoundException("Chat with this id not found");
       // chat.Users.Remove(userId);
        await _dbContext.SaveChangesAsync();
    }
//TODO 
    public async Task LeaveGroupChat(Guid userId, Guid chatId) {
        throw new NotImplementedException();
    }

    public async Task CreatePrivateChat(ChatCreateDto chatModel) {
        var users = await _dbContext.Users
            .Where(u => chatModel.Users
                .Contains(u.Id))
            .ToListAsync();
        if (chatModel.Users == null || users.Count != 2)
            throw new BadRequestException("private chat can exist only with 2 users");
        var chat = new PrivateChat {
            ChatAvatarId = chatModel.AvatarId,
            ChatName = chatModel.ChatName,
            Users = users,
        };
        await _dbContext.AddAsync(chat);
        await _dbContext.SaveChangesAsync();
    }

    public async Task CreateGroupChat(ChatCreateDto chatModel, Guid creatorId) {
        var admin = await _dbContext.Users
            .FirstOrDefaultAsync(u => u.Id == creatorId);
        if (admin == null) 
            throw new NotFoundException("creator with this id does not found");
        var users = await _dbContext.Users
            .Where(u => chatModel.Users
                .Contains(u.Id))
            .ToListAsync();
        if (chatModel.Users == null || users.Count < 2 )
            throw new BadRequestException("Group chat can exist only with more than 1 user");
        var chat = new GroupChat {
            ChatAvatarId = chatModel.AvatarId,
            ChatName = chatModel.ChatName,
            Users = users
        };
        chat.Administrators.Add(admin);
        await _dbContext.AddAsync(chat);
        await _dbContext.SaveChangesAsync();
    }

    public async Task AddUserToGroupChat(ChatUserActionsDto model) {
        var user = await _dbContext.Users
            .FirstOrDefaultAsync(u => u.Id == model.UserId);
        if (user == null) 
            throw new NotFoundException("user with this id does not found");
        var chat = await _dbContext.Chats
            .FirstOrDefaultAsync(c => c.Id == model.ChatId);
        if (chat == null) 
            throw new NotFoundException("Chat with this id not found");
        user.Chats.Add(chat);
        await _dbContext.SaveChangesAsync();
    }

    public async Task DeleteUserFromGroupChat(ChatUserActionsDto model) {
        var chat = await _dbContext.Chats
            .FirstOrDefaultAsync(c => c.Id == model.ChatId);
        if (chat == null) throw new NotFoundException("Chat with this id not found");
        var user = await _dbContext.Users
            .FirstOrDefaultAsync(u => u.Id == model.UserId);
        if (user == null) throw new NotFoundException("user with this id does not found");
        if (!user.Chats.Contains(chat))
            throw new BadRequestException("User with this Id does not contains in this chat");
        user.Chats.Remove(chat);
        await _dbContext.SaveChangesAsync();
    }

    public async Task EditChat(ChatEditDto model, Guid chatId) {
        var chat = await _dbContext.Chats
            .FirstOrDefaultAsync(c => c.Id == chatId);
        if (chat == null) throw new NotFoundException("Chat with this id not found");
        chat.ChatAvatarId = model.AvatarId;
        chat.ChatName = model.ChatName;
        await _dbContext.SaveChangesAsync();
    }

    public async Task DeleteGroupChat(Guid chatId) {
        var chat = await _dbContext.GroupChats
            .FirstOrDefaultAsync(c => c.Id == chatId);
        if (chat == null) throw new NotFoundException("Group chat with this id not found");
        chat.DeletedTime = DateTime.UtcNow;
        await _dbContext.SaveChangesAsync();
    }
}