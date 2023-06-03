using ChatNet.Common.DataTransferObjects;

namespace ChatNet.Common.Interfaces; 

public interface IChatService {
    public Task<ChatListDto> GetChatList(Guid userId);
    public Task<ChatFullDto> GetChatWithMessages(Guid chatId);
    public Task LeavePrivateChat(Guid userId, Guid chatId);
    public Task LeaveGroupChat(Guid userId, Guid chatId);
    public Task CreatePrivateChat(ChatCreateDto chatModel);
    public Task CreateGroupChat(ChatCreateDto chatModel, Guid creatorId);
    public Task AddUserToGroupChat(ChatUserActionsDto model);
    public Task DeleteUserFromGroupChat(ChatUserActionsDto model);
    public Task EditChat(ChatEditDto model, Guid chatId);
    public Task DeleteGroupChat(Guid chatId);
}