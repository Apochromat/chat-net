using ChatNet.Common.DataTransferObjects;
using ChatNet.Common.Exceptions;
using ChatNet.Common.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace ChatNet.Auth.API.Controllers; 

/// <summary>
/// Account controller
/// </summary>
[ApiController]
[Route("api/account")]
public class AccountController : ControllerBase{
    private readonly IAccountService _accountService;

    /// <summary>
    /// Constructor
    /// </summary>
    /// <param name="accountService"></param>
    public AccountController(IAccountService accountService) {
        _accountService = accountService;
    }
    
    /// <summary>
    /// Get information about current authenticated user
    /// </summary>
    /// <returns></returns>
    [HttpGet]
    [Authorize(AuthenticationSchemes = "Bearer")]
    public async Task<ActionResult<ProfileFullDto>> GetCurrentProfile() {
        if (User.Identity == null || Guid.TryParse(User.Identity.Name, out Guid userId) == false) {
            throw new UnauthorizedException("User is not authorized");
        }
        return Ok(await _accountService.GetProfileAsync(userId));
    }
    
    /// <summary> 
    /// Edit current authenticated user`s profile
    /// </summary>
    /// <returns></returns>
    [HttpPut]
    [Authorize(AuthenticationSchemes = "Bearer")]
    public async Task<ActionResult> UpdateProfile([FromBody] ProfileEditDto profileEditDto) {
        if (User.Identity == null || Guid.TryParse(User.Identity.Name, out Guid userId) == false) {
            throw new UnauthorizedException("User is not authorized");
        }
        
        await _accountService.EditProfileAsync(userId, profileEditDto);
        return Ok();
    }
    
    /// <summary>
    /// Get short information about current authenticated user
    /// </summary>
    /// <returns></returns>
    [HttpGet]
    [Route("{userId}")]
    public async Task<ActionResult<ProfileShortDto>> GetCurrentProfile(Guid userId) {
        return Ok(await _accountService.GetShortProfileAsync(userId));
    }

    /// <summary>
    /// Search users for adding to friends
    /// </summary>
    /// <param name="fullname"></param>
    /// <param name="page"></param>
    /// <param name="pageSize"></param>
    /// <returns></returns>
    [HttpGet]
    [Authorize(AuthenticationSchemes = "Bearer")]
    [Route("users")]
    public async Task<ActionResult<ProfileShortDto>> GetUsers([FromQuery] string? fullname, 
        [FromQuery] int page = 1, 
        [FromQuery] int pageSize = 15) {
        if (User.Identity == null || Guid.TryParse(User.Identity.Name, out Guid userId) == false) {
            throw new UnauthorizedException("User is not authorized");
        }
        return Ok(await _accountService.SearchUsersAsync(userId, fullname, page, pageSize));
    }
    
    /// <summary>
    /// Get information about user's 
    /// </summary>
    /// <param name="userIds"></param>
    /// <param name="fullname"></param>
    /// <returns></returns>
    /// <exception cref="UnauthorizedException"></exception>
    [HttpGet]
    [Authorize(AuthenticationSchemes = "Bearer")]
    [Route("userList")]
    public async Task<ActionResult<ProfileShortDto>> GetUsersByIds([FromQuery] List<Guid> userIds,
        [FromQuery] string? fullname
    ) {
        if (User.Identity == null || Guid.TryParse(User.Identity.Name, out Guid userId) == false) {
            throw new UnauthorizedException("User is not authorized");
        }
        return Ok(await _accountService.GetUsersByList(userIds, fullname));
    }
}