using ChatNet.Common.DataTransferObjects;

namespace ChatNet.Common.Interfaces; 

/// <summary>
/// Friend service for friend management 
/// </summary>
public interface IFriendService {
    /// <summary>
    /// Add friend 
    /// </summary>
    /// <param name="userId"></param>
    /// <param name="friendId"></param>
    /// <returns></returns>
    public Task AddFriend(Guid userId, Guid friendId);

    /// <summary>
    /// Get list friends
    /// </summary>
    /// <param name="userId"></param>
    /// <param name="page"></param>
    /// <param name="pageSize"></param>
    /// <returns></returns>
    public Task<Pagination<Guid>> GetFriends(Guid userId, int page, int pageSize);
    /// <summary>
    /// Delete friend
    /// </summary>
    /// <param name="userId"></param>
    /// <param name="friendId"></param>
    /// <returns></returns>
    public Task DeleteFriend(Guid userId, Guid friendId);
}