using ChatNet.Common.DataTransferObjects;
using ChatNet.Common.Exceptions;
using ChatNet.Common.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace ChatNet.Auth.API.Controllers;

/// <summary>
/// Controller for register, authentication, changing the password
/// </summary>
[ApiController]
[Route("api/auth")]
public class AuthController : ControllerBase {
    private readonly IAuthService _authService;

    /// <summary>
    /// Constructor
    /// </summary>
    /// <param name="authService"></param>
    public AuthController(IAuthService authService) {
        _authService = authService;
    }

    /// <summary>
    /// Register new user as Customer
    /// </summary>
    /// <returns></returns>
    [HttpPost]
    [Route("register")]
    public async Task<ActionResult<TokenResponseDto>> Register([FromBody] AccountRegisterDto accountRegisterDto) {
        return Ok(await _authService.RegisterAsync(accountRegisterDto, HttpContext));
    }

    /// <summary>
    /// Login user into the system
    /// </summary>
    /// <remarks>
    /// Returns the user access-token(JWT) and refresh-token(non-JWT).<br/>
    /// If the user logs in from any device, this device is remembered to work with a specific refresh-token
    /// </remarks>
    /// <returns></returns>
    [HttpPost]
    [Route("login")]
    public async Task<ActionResult<TokenResponseDto>> Login([FromBody] AccountLoginDto accountLoginDto) {
        return Ok(await _authService.LoginAsync(accountLoginDto, HttpContext));
    }

    /// <summary>
    /// Refreshes access-token
    /// </summary>
    /// <returns></returns>
    [HttpPost]
    [Route("refresh")]
    public async Task<ActionResult<TokenResponseDto>> Refresh([FromBody] TokenRequestDto tokenRequestDto) {
        return Ok(await _authService.RefreshTokenAsync(tokenRequestDto, HttpContext));
    }
    
    /// <summary>
    /// Logout user by deleting his current device
    /// </summary>
    /// <returns></returns>
    /// <exception cref="UnauthorizedException"></exception>
    [HttpPost]
    [Authorize(AuthenticationSchemes = "Bearer")]
    [Route("logout")]
    public async Task<ActionResult> Logout() {
        if (User.Identity == null || Guid.TryParse(User.Identity.Name, out Guid userId) == false) {
            throw new UnauthorizedException("User is not authorized");
        }
        
        await _authService.LogoutAsync(userId, HttpContext);
        return Ok();
    }

    /// <summary>
    /// Changes user password
    /// </summary>
    /// <returns></returns>
    [HttpPut]
    [Authorize(AuthenticationSchemes = "Bearer")]
    [Route("change-password")]
    public async Task<ActionResult> ChangePassword([FromBody] ChangePasswordDto changePasswordDto) {
        if (User.Identity == null || Guid.TryParse(User.Identity.Name, out Guid userId) == false) {
            throw new UnauthorizedException("User is not authorized");
        }

        await _authService.ChangePasswordAsync(userId, changePasswordDto);
        return Ok();
    }

    /// <summary>
    /// Get user devices
    /// </summary>
    /// <returns></returns>
    [HttpGet]
    [Authorize(AuthenticationSchemes = "Bearer")]
    [Route("devices")]
    public async Task<ActionResult<List<DeviceDto>>> GetDevices() {
        if (User.Identity == null || Guid.TryParse(User.Identity.Name, out Guid userId) == false) {
            throw new UnauthorizedException("User is not authorized");
        }

        return Ok(await _authService.GetDevicesAsync(userId));
    }

    /// <summary>
    /// Rename device
    /// </summary>
    /// <param name="deviceId"></param>
    /// <param name="deviceRenameDto"></param>
    /// <returns></returns>
    [HttpPut]
    [Authorize(AuthenticationSchemes = "Bearer")]
    [Route("devices/{deviceId}")]
    public async Task<ActionResult> RenameDevice([FromRoute] Guid deviceId, [FromBody] DeviceRenameDto deviceRenameDto) {
        if (User.Identity == null || Guid.TryParse(User.Identity.Name, out Guid userId) == false) {
            throw new UnauthorizedException("User is not authorized");
        }

        await _authService.RenameDeviceAsync(userId, deviceId, deviceRenameDto);
        return Ok();
    }

    /// <summary>
    /// Delete device from user devices
    /// </summary>
    /// <param name="deviceId"></param>
    /// <returns></returns>
    [HttpDelete]
    [Authorize(AuthenticationSchemes = "Bearer")]
    [Route("devices/{deviceId}")]
    public async Task<ActionResult> DeleteDevice([FromRoute] Guid deviceId) {
        if (User.Identity == null || Guid.TryParse(User.Identity.Name, out Guid userId) == false) {
            throw new UnauthorizedException("User is not authorized");
        }

        await _authService.DeleteDeviceAsync(userId, deviceId);
        return Ok();
    }
}