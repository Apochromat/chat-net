using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace ChatNet.Call.API.Controllers;

/// <summary>
/// Controller for call management
/// </summary>
[ApiController]
[Route("api/call")]
[Authorize(AuthenticationSchemes = "Bearer")]
public class CallController : ControllerBase {
    private readonly ILogger<CallController> _logger;

    /// <summary>
    /// Constructor
    /// </summary>
    /// <param name="logger"></param>
    public CallController(ILogger<CallController> logger) {
        _logger = logger;
    }

    /// <summary>
    /// Call somebody
    /// </summary>
    /// <returns></returns>
    [HttpPost]
    public async Task<ActionResult> CallSomebody() {
        return Ok();
    }

    /// <summary>
    /// Get list of current calls
    /// </summary>
    /// <returns></returns>
    [HttpGet]
    public async Task<ActionResult> GetCurrentCalls() {
        return Ok();
    }

    /// <summary>
    /// Accept call
    /// </summary>
    /// <param name="callId"></param>
    /// <returns></returns>
    [HttpPost]
    [Route("{callId}/accept")]
    public async Task<ActionResult> AcceptCall([FromRoute] Guid callId) {
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
        return Ok();
    }
}