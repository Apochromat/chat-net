using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace ChatNet.Call.API.Controllers;

/// <summary>
/// Controller for call preferences management
/// </summary>
[ApiController]
[Route("api/call/preferences")]
[Authorize(AuthenticationSchemes = "Bearer")]
public class PreferencesController : ControllerBase {
    private readonly ILogger<PreferencesController> _logger;

    /// <summary>
    /// Constructor
    /// </summary>
    /// <param name="logger"></param>
    public PreferencesController(ILogger<PreferencesController> logger) {
        _logger = logger;
    }

    /// <summary>
    /// Get call preferences
    /// </summary>
    /// <returns></returns>
    [HttpGet]
    public async Task<ActionResult> GetPreferences() {
        return Ok();
    }

    /// <summary>
    /// Set call preferences
    /// </summary>
    /// <returns></returns>
    [HttpPut]
    public async Task<ActionResult> SetPreferences() {
        return Ok();
    }
}