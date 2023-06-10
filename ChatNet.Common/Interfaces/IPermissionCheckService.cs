using Microsoft.AspNetCore.SignalR;

namespace ChatNet.Common.Interfaces; 

/// <summary>
/// Permission service
/// </summary>
public interface IPermissionCheckService {
    /// <summary>
    /// Check user is admin
    /// </summary>
    /// <param name="userId"></param>
    /// <param name="chatId"></param>
    /// <returns></returns>
    public Task CheckUserIsChatAdmin(Guid userId, Guid chatId);
    /// <summary>
    /// Check user access to chat
    /// </summary>
    /// <param name="userId"></param>
    /// <param name="chatId"></param>
    /// <returns></returns>
    public Task CheckUserHasAccessToChat(Guid userId, Guid chatId);
}