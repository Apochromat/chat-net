using Microsoft.AspNetCore.SignalR;

namespace ChatNet.Common.Interfaces; 

public interface IPermissionCheckService {
    public Task CheckUserIsChatAdmin(Guid UserId, Guid ChatId);
    public Task CheckUserHasAccessToChat(Guid UserId, Guid ChatId);
}