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
    /// Constructor
    /// </summary>
    /// <param name="friendService"></param>
    public FriendController(IFriendService friendService) {
        _friendService = friendService;
    }
    
    /// <summary>
    /// Get user's friend paginated list
    /// </summary>
    /// <param name="page"></param>
    /// <param name="pageSize"></param>
    /// <returns></returns>
    /// <exception cref="UnauthorizedException"></exception>
    [HttpGet]
    [Route("friends")]
    [Authorize(AuthenticationSchemes = "Bearer")]
    public async Task<ActionResult<Pagination<Guid>>> GetFriends([FromQuery] int page = 1,
        [FromQuery] int pageSize = 15) {
        if (User.Identity == null || Guid.TryParse(User.Identity.Name, out Guid userId) == false) {
            throw new UnauthorizedException("User is not authorized");
        }
        return Ok(await _friendService.GetFriends(userId, page,pageSize));
    }

    /// <summary>
    /// Get user's friendship requests
    /// </summary>
    /// <remarks>
    /// If myRequest = true you get requests sent by yourself , else get requests sent for you
    /// </remarks>
    /// <param name="myRequests"></param>
    /// <param name="page"></param>
    /// <param name="pageSize"></param>
    /// <returns></returns>
    /// <exception cref="UnauthorizedException"></exception>
    [HttpGet]
    [Route("friendship/requests")]
    [Authorize(AuthenticationSchemes = "Bearer")]
    public async Task<ActionResult<Pagination<Guid>>> GetRequests([FromQuery] bool myRequests = true,[FromQuery] int page = 1,
        [FromQuery] int pageSize = 15) {
        if (User.Identity == null || Guid.TryParse(User.Identity.Name, out Guid userId) == false) {
            throw new UnauthorizedException("User is not authorized");
        } 
        return Ok(await _friendService.GetUserFriendShipRequests(userId , myRequests , page , pageSize));
    }
    
    /// <summary>
    /// Send friendship request
    /// </summary>
    /// <param name="friendId"></param>
    /// <returns></returns>
    /// <exception cref="UnauthorizedException"></exception>
    [HttpPost]
    [Route("friend/{friendId}")]
    [Authorize(AuthenticationSchemes = "Bearer")]
    public async Task<ActionResult> SendFriendShipRequest(Guid friendId) {
        if (User.Identity == null || Guid.TryParse(User.Identity.Name, out Guid userId) == false) {
            throw new UnauthorizedException("User is not authorized");
        } 
        await _friendService.SendFriendshipRequest(userId, friendId);
        return Ok();
    }

    /// <summary>
    /// Accept friendship request
    /// </summary>
    /// <param name="friendId"></param>
    /// <returns></returns>
    /// <exception cref="UnauthorizedException"></exception>
    [HttpPost]
    [Route("friendship/request/{friendId}")]
    [Authorize(AuthenticationSchemes = "Bearer")]
    public async Task<ActionResult> AcceptFriendShipRequest(Guid friendId) {
        if (User.Identity == null || Guid.TryParse(User.Identity.Name, out Guid userId) == false) {
            throw new UnauthorizedException("User is not authorized");
        } 
        await _friendService.AcceptFriendshipRequest(userId, friendId);
        return Ok();
    }

    /// <summary>
    /// Reject friendship request
    /// </summary>
    /// <param name="friendId"></param>
    /// <returns></returns>
    /// <exception cref="UnauthorizedException"></exception>
    [HttpDelete]
    [Route("friendship/request/{friendId}")]
    [Authorize(AuthenticationSchemes = "Bearer")]
    public async Task<ActionResult> RejectFriendshipRequest(Guid friendId) {
        if (User.Identity == null || Guid.TryParse(User.Identity.Name, out Guid userId) == false) {
            throw new UnauthorizedException("User is not authorized");
        }
        await _friendService.RejectFriendshipRequest(userId, friendId);
        return Ok();
    }
    
    /// <summary>
    /// Delete friend
    /// </summary>
    /// <param name="friendId"></param>
    /// <returns></returns>
    /// <exception cref="UnauthorizedException"></exception>
    [HttpDelete]
    [Route("friend/{friendId}")]
    [Authorize(AuthenticationSchemes = "Bearer")]
    public async Task<ActionResult> DeleteFriend(Guid friendId) {
        if (User.Identity == null || Guid.TryParse(User.Identity.Name, out Guid userId) == false) {
            throw new UnauthorizedException("User is not authorized");
        }
        await _friendService.DeleteFriend(userId, friendId);
        return Ok();
    }
}