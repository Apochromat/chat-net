using ChatNet.Backend.DAL;
using ChatNet.Common.Exceptions;
using ChatNet.Common.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace ChatNet.Backend.BLL.Services; 

public class PermissionCheckService:IPermissionCheckService {
    private readonly BackendDbContext _dbContext;

    public PermissionCheckService(BackendDbContext dbContext) {
        _dbContext = dbContext;
    }

    public async Task CheckUserIsChatAdmin(Guid userId, Guid chatId) {
        var user = await _dbContext.Users
            .FirstOrDefaultAsync(u => u.Id == userId);
        if (user == null) 
            throw new NotFoundException("user with this id does not found");
        var chat = await _dbContext.GroupChats
            .Include(c=>c.Administrators)
            .Include(c=>c.Users)
            .FirstOrDefaultAsync(c => c.Id == chatId);
        if (chat == null) 
            throw new NotFoundException("Chat with this id not found");
        if (!chat.Administrators.Contains(user) || !chat.Users.Contains(user))
            throw new MethodNotAllowedException("you don't have permission");
    }

    public async Task CheckUserHasAccessToChat(Guid userId, Guid chatId) {
        var user = await _dbContext.Users
            .FirstOrDefaultAsync(u => u.Id == userId);
        if (user == null) 
            throw new NotFoundException("user with this id does not found");
        var chat = await _dbContext.Chats
            .Include(c=>c.Users)
            .FirstOrDefaultAsync(c => c.Id == chatId);
        if (chat == null) 
            throw new NotFoundException("Chat with this id not found");
        if (!chat.Users.Contains(user))
            throw new MethodNotAllowedException("you don't have access to this chat");
    }
}