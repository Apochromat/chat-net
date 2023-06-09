using ChatNet.Common.DataTransferObjects;

namespace ChatNet.Common.Interfaces; 

/// <summary>
/// Friend service for friend management 
/// </summary>
public interface IFriendService {
    /// <summary>
    /// Accept request 
    /// </summary>
    /// <param name="userId"></param>
    /// <param name="friendId"></param>
    /// <returns></returns>
    public Task AcceptFriendshipRequest(Guid userId, Guid friendId);

    /// <summary>
    /// Reject request
    /// </summary>
    /// <param name="userId"></param>
    /// <param name="friendId"></param>
    /// <returns></returns>
    public Task RejectFriendshipRequest(Guid userId, Guid friendId);

    /// <summary>
    /// Get requests
    /// </summary>
    /// <param name="userId"></param>
    /// <param name="myRequests"></param>
    /// <param name="page"></param>
    /// <param name="pageSize"></param>
    /// <returns></returns>
    public Task<Pagination<Guid>> GetUserFriendShipRequests(Guid userId, bool myRequests,
        int page , int pageSize);

    /// <summary>
    /// Send request
    /// </summary>
    /// <param name="userId"></param>
    /// <param name="friendId"></param>
    /// <returns></returns>
    public Task SendFriendshipRequest(Guid userId, Guid friendId);

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