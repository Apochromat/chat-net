using ChatNet.Common.DataTransferObjects;
using ChatNet.Common.Exceptions;
using ChatNet.Common.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace ChatNet.Notification.API.Controllers; 

/// <summary>
/// Controller for notification service
/// </summary>
[ApiController]
[Route("api/notification")]
[Authorize (AuthenticationSchemes = "Bearer")]
public class NotificationController : Controller {
    private readonly IOnlinePreferencesManagerService _onlinePreferencesManagerService;

    /// <summary>
    /// Constructor
    /// </summary>
    /// <param name="onlinePreferencesManagerService"></param>
    public NotificationController(IOnlinePreferencesManagerService onlinePreferencesManagerService) {
        _onlinePreferencesManagerService = onlinePreferencesManagerService;
    }
    
    /// <summary>
    /// Get current user's online preferences
    /// </summary>
    /// <returns></returns>
    /// <exception cref="UnauthorizedException"></exception>
    /// <exception cref="NotFoundException"></exception>
    [HttpGet]
    [Route("online-preferences")]
    public async Task<ActionResult<OnlinePreferenceFullDto>> GetMyOnlinePreferences() {
        if (User.Identity == null || Guid.TryParse(User.Identity.Name, out Guid userId) == false) {
            throw new UnauthorizedException("User is not authorized");
        }
        
        var preference = await _onlinePreferencesManagerService.GetPreferenceAsync(userId);
        if (preference == null) {
            throw new NotFoundException("Online preferences not found");
        }

        return Ok(preference);
    }
    
    /// <summary>
    /// Set current user's online preferences
    /// </summary>
    /// <returns></returns>
    /// <exception cref="UnauthorizedException"></exception>
    /// <exception cref="NotFoundException"></exception>
    [HttpPut]
    [Route("online-preferences")]
    public async Task<ActionResult> SetMyOnlinePreferences([FromBody] OnlinePreferenceTypeDto preferenceTypeDto) {
        if (User.Identity == null || Guid.TryParse(User.Identity.Name, out Guid userId) == false) {
            throw new UnauthorizedException("User is not authorized");
        }
        
        await _onlinePreferencesManagerService.SetPreferenceTypeAsync(userId, preferenceTypeDto);

        return Ok();
    }
}