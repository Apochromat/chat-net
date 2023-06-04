using Microsoft.AspNetCore.SignalR;

namespace ChatNet.Common.Interfaces; 

public interface IPermissionCheckService {
    public Task CheckUserIsChatAdmin(Guid userId, Guid chatId);
    public Task CheckUserHasAccessToChat(Guid userId, Guid chatId);
}