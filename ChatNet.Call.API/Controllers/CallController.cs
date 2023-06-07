using Microsoft.AspNetCore.Mvc;

namespace ChatNet.Call.API.Controllers;

/// <summary>
/// Controller for call management
/// </summary>
[ApiController]
[Route("api/call")]
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
    public async Task<ActionResult> CallSomebody () {
        return Ok();
    }
}