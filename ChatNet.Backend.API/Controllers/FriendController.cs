using ChatNet.Common.DataTransferObjects;
using ChatNet.Common.Exceptions;
using ChatNet.Common.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace ChatNet.Backend.API.Controllers; 

/// <summary>
/// Friend controller
/// </summary>
[ApiController]
[Route("api/backend")]
public class FriendController: ControllerBase {
    private readonly IFriendService _friendService;

    /// <summary>
    /// constructor
    /// </summary>
    /// <param name="friendService"></param>
    public FriendController(IFriendService friendService) {
        _friendService = friendService;
    }
    
    /// <summary>
    /// get user's friend paginated list
    /// </summary>
    /// <param name="page"></param>
    /// <param name="pageSize"></param>
    /// <returns></returns>
    /// <exception cref="UnauthorizedException"></exception>
    [HttpGet]
    [Route("friends")]
    [Authorize(AuthenticationSchemes = "Bearer")]
    public async Task<ActionResult<ChatListDto>> GetFriends([FromQuery] int page = 1,
        [FromQuery] int pageSize = 15) {
        if (User.Identity == null || Guid.TryParse(User.Identity.Name, out Guid userId) == false) {
            throw new UnauthorizedException("User is not authorized");
        }
        return Ok(await _friendService.GetFriends(userId, page,pageSize));
    }

    /// <summary>
    /// add friend
    /// </summary>
    /// <param name="page"></param>
    /// <param name="pageSize"></param>
    /// <param name="friendId"></param>
    /// <returns></returns>
    /// <exception cref="UnauthorizedException"></exception>
    [HttpPost]
    [Route("friend/{friendId}")]
    [Authorize(AuthenticationSchemes = "Bearer")]
    public async Task<ActionResult<ChatListDto>> AddFriend(Guid friendId) {
        if (User.Identity == null || Guid.TryParse(User.Identity.Name, out Guid userId) == false) {
            throw new UnauthorizedException("User is not authorized");
        }
        await _friendService.AddFriend(userId, friendId);
        return Ok();
    }
    
    /// <summary>
    /// delete friend
    /// </summary>
    /// <param name="friendId"></param>
    /// <returns></returns>
    /// <exception cref="UnauthorizedException"></exception>
    [HttpDelete]
    [Route("friend/{friendId}")]
    [Authorize(AuthenticationSchemes = "Bearer")]
    public async Task<ActionResult<ChatListDto>> DeleteFriend(Guid friendId) {
        if (User.Identity == null || Guid.TryParse(User.Identity.Name, out Guid userId) == false) {
            throw new UnauthorizedException("User is not authorized");
        }
        await _friendService.DeleteFriend(userId, friendId);
        return Ok();
    }
}