using ChatNet.Common.DataTransferObjects;
using ChatNet.Common.Enumerations;

namespace ChatNet.Common.Interfaces; 

/// <summary>
/// Chat service
/// </summary>
public interface IChatService {
    /// <summary>
    /// Get chat list
    /// </summary>
    /// <param name="userId"></param>
    /// <param name="page"></param>
    /// <param name="pageSize"></param>
    /// <returns></returns>
    public Task<ChatListDto> GetChatList(Guid userId, int page, int pageSize);
    /// <summary>
    /// Get chat messages
    /// </summary>
    /// <param name="chatId"></param>
    /// <param name="page"></param>
    /// <param name="pageSize"></param>
    /// <returns></returns>
    public Task<Pagination<MessageDto>> GetMessages(Guid chatId, int page, int pageSize);
    /// <summary>
    /// Get chat details
    /// </summary>
    /// <param name="chatId"></param>
    /// <returns></returns>
    public Task<ChatFullDto> GetChatDetails(Guid chatId);
    /// <summary>
    /// Leave private chat
    /// </summary>
    /// <param name="userId"></param>
    /// <param name="chatId"></param>
    /// <returns></returns>
    public Task LeavePrivateChat(Guid userId, Guid chatId);
    /// <summary>
    /// Leave group chat
    /// </summary>
    /// <param name="userId"></param>
    /// <param name="chatId"></param>
    /// <returns></returns>
    public Task LeaveGroupChat(Guid userId, Guid chatId);
    /// <summary>
    /// Create private chat
    /// </summary>
    /// <param name="chatModel"></param>
    /// <param name="creatorId"></param>
    /// <returns></returns>
    public Task CreatePrivateChat(ChatPrivateCreateDto chatModel , Guid creatorId);
    /// <summary>
    /// Create group chat
    /// </summary>
    /// <param name="chatModel"></param>
    /// <param name="creatorId"></param>
    /// <returns></returns>
    public Task CreateGroupChat(ChatCreateDto chatModel, Guid creatorId);

    /// <summary>
    /// Add user to group chat
    /// </summary>
    /// <param name="chatId"></param>
    /// <param name="userId"></param>
    /// <param name="adminId"></param>
    /// <returns></returns>
    public Task AddUserToGroupChat(Guid chatId , Guid userId, Guid adminId);
    /// <summary>
    /// Delete user from group chat
    /// </summary>
    /// <param name="chatId"></param>
    /// <param name="userId"></param>
    /// <returns></returns>
    public Task DeleteUserFromGroupChat(Guid chatId , Guid userId);
    /// <summary>
    /// Edit chat
    /// </summary>
    /// <param name="model"></param>
    /// <param name="chatId"></param>
    /// <returns></returns>
    public Task EditChat(ChatEditDto model, Guid chatId);
    /// <summary>
    /// Delete group chat
    /// </summary>
    /// <param name="chatId"></param>
    /// <param name="adminId"></param>
    /// <returns></returns>
    public Task DeleteGroupChat(Guid chatId, Guid adminId);
    /// <summary>
    /// Make user admin
    /// </summary>
    /// <param name="chatId"></param>
    /// <param name="adminId"></param>
    /// <param name="newAdminId"></param>
    /// <returns></returns>
    public Task MakeUserAdmin(Guid chatId, Guid adminId, Guid newAdminId);
    /// <summary>
    /// Edit notification preference
    /// </summary>
    /// <param name="chatId"></param>
    /// <param name="userId"></param>
    /// <param name="preferenceType"></param>
    /// <returns></returns>
    public Task EditNotificationsPreference(Guid chatId, Guid userId, NotificationPreferenceType preferenceType);

    /// <summary>
    /// Get preference
    /// </summary>
    /// <param name="chatId"></param>
    /// <param name="userId"></param>
    /// <returns></returns>
    public Task<NotificationPreferenceType> GetNotificationPreference(Guid chatId, Guid userId);
}