using ChatNet.Common.DataTransferObjects;

namespace ChatNet.Common.Interfaces; 

public interface IChatService {
    public Task<ChatListDto> GetChatList(Guid userId, int page, int pageSize);
    public Task<ChatFullDto> GetChatWithMessages(Guid chatId, int page, int pageSize);
    public Task LeavePrivateChat(Guid userId, Guid chatId);
    public Task LeaveGroupChat(Guid userId, Guid chatId);
    public Task CreatePrivateChat(ChatPrivateCreateDto chatModel , Guid creatorId);
    public Task CreateGroupChat(ChatCreateDto chatModel, Guid creatorId);
    public Task AddUserToGroupChat(Guid chatId , Guid userId);
    public Task DeleteUserFromGroupChat(Guid chatId , Guid userId);
    public Task EditChat(ChatEditDto model, Guid chatId);
    public Task DeleteGroupChat(Guid chatId, Guid adminId);
    public Task MakeUserAdmin(Guid chatId, Guid adminId, Guid newAdminId);
}