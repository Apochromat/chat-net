using ChatNet.Call.API.Hubs;
using ChatNet.Common.Exceptions;
using ChatNet.Common.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.SignalR;

namespace ChatNet.Call.API.Controllers;

/// <summary>
/// Controller for call management
/// </summary>
[ApiController]
[Route("api/call")]
[Authorize(AuthenticationSchemes = "Bearer")]
public class CallController : ControllerBase {
    private readonly ILogger<CallController> _logger;
    private readonly ICallService _callService;
    private readonly IHubContext<CallHub> _callHubContext;

    /// <summary>
    /// Constructor
    /// </summary>
    /// <param name="logger"></param>
    /// <param name="callService"></param>
    /// <param name="callHubContext"></param>
    public CallController(ILogger<CallController> logger, ICallService callService, IHubContext<CallHub> callHubContext) {
        _logger = logger;
        _callService = callService;
        _callHubContext = callHubContext;
    }

    /// <summary>
    /// Call somebody
    /// </summary>
    /// <returns></returns>
    [HttpPost]
    [Route("{receiverId}")]
    public async Task<ActionResult<Guid>> CallSomebody([FromRoute] Guid receiverId) {
        if (User.Identity == null || Guid.TryParse(User.Identity.Name, out Guid userId) == false) {
            throw new UnauthorizedException("User is not authorized");
        }

        var callId = await _callService.CallSomebody(userId, receiverId);
        await _callHubContext.Clients.User(userId.ToString()).SendAsync("created", callId);
        return Ok(callId);
    }

    /// <summary>
    /// Get list of current calls
    /// </summary>
    /// <returns></returns>
    [HttpGet]
    public async Task<ActionResult> GetCurrentCalls() {
        if (User.Identity == null || Guid.TryParse(User.Identity.Name, out Guid userId) == false) {
            throw new UnauthorizedException("User is not authorized");
        }
        
        return Ok(await _callService.GetCurrentCalls(userId));
    }

    /// <summary>
    /// Accept call
    /// </summary>
    /// <param name="callId"></param>
    /// <returns></returns>
    [HttpPost]
    [Route("{callId}/accept")]
    public async Task<ActionResult> AcceptCall([FromRoute] Guid callId) {
        if (User.Identity == null || Guid.TryParse(User.Identity.Name, out Guid userId) == false) {
            throw new UnauthorizedException("User is not authorized");
        }

        await _callService.AcceptCall(callId, userId);
        return Ok();
    }

    /// <summary>
    /// Reject call as receiver
    /// </summary>
    /// <param name="callId"></param>
    /// <returns></returns>
    [HttpPost]
    [Route("{callId}/reject")]
    public async Task<ActionResult> RejectCall([FromRoute] Guid callId) {
        if (User.Identity == null || Guid.TryParse(User.Identity.Name, out Guid userId) == false) {
            throw new UnauthorizedException("User is not authorized");
        }

        await _callService.RejectCall(callId, userId);
        return Ok();
    }
    
    /// <summary>
    /// Cancel call as caller
    /// </summary>
    /// <param name="callId"></param>
    /// <returns></returns>
    [HttpPost]
    [Route("{callId}/cancel")]
    public async Task<ActionResult> CancelCall([FromRoute] Guid callId) {
        if (User.Identity == null || Guid.TryParse(User.Identity.Name, out Guid userId) == false) {
            throw new UnauthorizedException("User is not authorized");
        }

        await _callService.CancelCall(callId, userId);
        return Ok();
    }

    /// <summary>
    /// Hangup call
    /// </summary>
    /// <param name="callId"></param>
    /// <returns></returns>
    [HttpPost]
    [Route("{callId}/hangup")]
    public async Task<ActionResult> HangupCall([FromRoute] Guid callId) {
        if (User.Identity == null || Guid.TryParse(User.Identity.Name, out Guid userId) == false) {
            throw new UnauthorizedException("User is not authorized");
        }

        await _callService.HangUpCall(callId, userId);
        return Ok();
    }
}