using ChatNet.Common.DataTransferObjects;
using ChatNet.Common.Exceptions;
using ChatNet.Common.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace ChatNet.Backend.API.Controllers;

[ApiController]
[Route("api/backend")]
public class ChatController : ControllerBase {
    private readonly ILogger<ChatController> _logger;
    private readonly IChatService _chatService;
    private readonly IPermissionCheckService _permissionCheckService;
    public ChatController(ILogger<ChatController> logger,
        IChatService chatService,
        IPermissionCheckService permissionCheckService) {
        _logger = logger;
        _chatService = chatService;
        _permissionCheckService = permissionCheckService;
    }

    /// <summary>
    /// Get chat's preview 
    /// </summary>
    /// <param name="page"></param>
    /// <param name="pageSize"></param>
    /// <returns></returns>
    /// <exception cref="UnauthorizedException"></exception>
    [HttpGet]
    [Route("/chats/preview")]
    [Authorize(AuthenticationSchemes = "Bearer")]
    public async Task<ActionResult<ChatListDto>> GetUserChats([FromQuery] int page = 1,
        [FromQuery] int pageSize = 15) {
        if (User.Identity == null || Guid.TryParse(User.Identity.Name, out Guid userId) == false) {
            throw new UnauthorizedException("User is not authorized");
        }
        return Ok(await _chatService.GetChatList(userId, page, pageSize));
    }
    
    /// <summary>
    /// Get all information about chat with paginated messages
    /// </summary>
    /// <param name="chatId"></param>
    /// <param name="page"></param>
    /// <param name="pageSize"></param>
    /// <returns></returns>
    /// <exception cref="UnauthorizedException"></exception>
    [HttpGet]
    [Route("/chat/{chatId}/messages")]
    [Authorize(AuthenticationSchemes = "Bearer")]
    public async Task<ActionResult<ChatFullDto>> GetChatWithMessages(Guid chatId, [FromQuery] int page = 1,
        [FromQuery] int pageSize = 15) {
        if (User.Identity == null || Guid.TryParse(User.Identity.Name, out Guid userId) == false) {
            throw new UnauthorizedException("User is not authorized");
        }

        await _permissionCheckService.CheckUserHasAccessToChat(userId, chatId);
        return Ok(await _chatService.GetChatWithMessages(chatId, page, pageSize));
    }
    
    /// <summary>
    /// Create private chat only for 2 person
    /// </summary>
    /// <param name="model"></param>
    /// <returns></returns>
    /// <exception cref="UnauthorizedException"></exception>
    [HttpPost]
    [Route("/chat/create/private")]
    [Authorize(AuthenticationSchemes = "Bearer")]
    public async Task<ActionResult> CreatePrivateChat([FromBody] ChatPrivateCreateDto model) {
        if (User.Identity == null || Guid.TryParse(User.Identity.Name, out Guid userId) == false) {
            throw new UnauthorizedException("User is not authorized");
        }

        await _chatService.CreatePrivateChat(model , userId);
        return Ok();
    }
    
    /// <summary>
    /// Create group chat
    /// </summary>
    /// <remarks>
    /// Creator is admin , chat can contains >=2 users
    /// </remarks>
    /// <param name="model"></param>
    /// <returns></returns>
    /// <exception cref="UnauthorizedException"></exception>
    [HttpPost]
    [Route("/chat/create/group")]
    [Authorize(AuthenticationSchemes = "Bearer")]
    public async Task<ActionResult> CreateGroupChat([FromBody] ChatCreateDto model) {
        if (User.Identity == null || Guid.TryParse(User.Identity.Name, out Guid userId) == false) {
            throw new UnauthorizedException("User is not authorized");
        }
        await _chatService.CreateGroupChat(model , userId);
        return Ok();
    }
    
    /// <summary>
    /// Edit chat
    /// </summary>
    /// <param name="model"></param>
    /// <param name="chatId"></param>
    /// <returns></returns>
    /// <exception cref="UnauthorizedException"></exception>
    [HttpPut]
    [Route("/chat/{chatId}")]
    [Authorize(AuthenticationSchemes = "Bearer")]
    public async Task<ActionResult> EditChat([FromBody] ChatEditDto model, Guid chatId) {
        if (User.Identity == null || Guid.TryParse(User.Identity.Name, out Guid userId) == false) {
            throw new UnauthorizedException("User is not authorized");
        }
        await _permissionCheckService.CheckUserHasAccessToChat(userId, chatId);
        await _chatService.EditChat(model , chatId);
        return Ok();
    }
    
    /// <summary>
    /// Delete chat 
    /// </summary>
    /// <param name="model"></param>
    /// <param name="chatId"></param>
    /// <returns></returns>
    /// <exception cref="UnauthorizedException"></exception>
    [HttpDelete]
    [Route("/chat/{chatId}/group")]
    [Authorize(AuthenticationSchemes = "Bearer")]
    public async Task<ActionResult> DeleteChat([FromBody] ChatEditDto model, Guid chatId) {
        if (User.Identity == null || Guid.TryParse(User.Identity.Name, out Guid userId) == false) {
            throw new UnauthorizedException("User is not authorized");
        }
        await _permissionCheckService.CheckUserIsChatAdmin(userId, chatId);
        await _chatService.DeleteGroupChat(chatId , userId);
        return Ok();
    }
    
    /// <summary>
    /// Leave private chat
    /// </summary>
    /// <param name="chatId"></param>
    /// <returns></returns>
    /// <exception cref="UnauthorizedException"></exception>
    [HttpDelete]
    [Route("/chat/{chatId}/leave/private")]
    [Authorize(AuthenticationSchemes = "Bearer")]
    public async Task<ActionResult> LeavePrivateChat(Guid chatId) {
        if (User.Identity == null || Guid.TryParse(User.Identity.Name, out Guid userId) == false) {
            throw new UnauthorizedException("User is not authorized");
        }
        await _permissionCheckService.CheckUserHasAccessToChat(userId, chatId);
        await _chatService.LeavePrivateChat(userId, chatId);
        return Ok();
    }
    
    /// <summary>
    /// Leave group chat
    /// </summary>
    /// <param name="chatId"></param>
    /// <returns></returns>
    /// <exception cref="UnauthorizedException"></exception>
    [HttpDelete]
    [Route("/chat/{chatId}/leave/group")]
    [Authorize(AuthenticationSchemes = "Bearer")]
    public async Task<ActionResult> LeaveGroupChat(Guid chatId) {
        if (User.Identity == null || Guid.TryParse(User.Identity.Name, out Guid userId) == false) {
            throw new UnauthorizedException("User is not authorized");
        }
        await _permissionCheckService.CheckUserHasAccessToChat(userId, chatId);
        await _chatService.LeaveGroupChat(userId, chatId);
        return Ok();
    }
    
    /// <summary>
    /// Add user in chat (only for admin)
    /// </summary>
    /// <param name="chatId"></param>
    /// <param name="userId"></param>
    /// <returns></returns>
    /// <exception cref="UnauthorizedException"></exception>
    [HttpPost]
    [Route("/chat/{chatId}/user/{userId}")]
    [Authorize(AuthenticationSchemes = "Bearer")]
    public async Task<ActionResult> AddUserToChat(Guid chatId, Guid userId) {
        if (User.Identity == null || Guid.TryParse(User.Identity.Name, out Guid adminId) == false) {
            throw new UnauthorizedException("User is not authorized");
        }
        await _permissionCheckService.CheckUserIsChatAdmin(adminId, chatId);
        await _chatService.AddUserToGroupChat(chatId , userId);
        return Ok();
    }
    
    /// <summary>
    /// Make user admin
    /// </summary>
    /// <param name="chatId"></param>
    /// <param name="userId"></param>
    /// <returns></returns>
    /// <exception cref="UnauthorizedException"></exception>
    [HttpPost]
    [Route("/chat/{chatId}/user/{userId}/admin")]
    [Authorize(AuthenticationSchemes = "Bearer")]
    public async Task<ActionResult> MakeUserAdmin(Guid chatId, Guid userId) {
        if (User.Identity == null || Guid.TryParse(User.Identity.Name, out Guid adminId) == false) {
            throw new UnauthorizedException("User is not authorized");
        }
        await _permissionCheckService.CheckUserIsChatAdmin(adminId, chatId);
        await _permissionCheckService.CheckUserHasAccessToChat(userId, chatId);
        await _chatService.MakeUserAdmin(chatId , adminId,userId);
        return Ok();
    }
    
    /// <summary>
    /// Delete user from chat (only for admin)
    /// </summary>
    /// <param name="chatId"></param>
    /// <param name="userId"></param>
    /// <returns></returns>
    /// <exception cref="UnauthorizedException"></exception>
    [HttpDelete]
    [Route("/chat/{chatId}/group/user/{userId}")]
    [Authorize(AuthenticationSchemes = "Bearer")]
    public async Task<ActionResult> DeleteUserFromChat(Guid chatId, Guid userId) {
        if (User.Identity == null || Guid.TryParse(User.Identity.Name, out Guid adminId) == false) {
            throw new UnauthorizedException("User is not authorized");
        }
        await _permissionCheckService.CheckUserIsChatAdmin(adminId, chatId);
        await _chatService.DeleteUserFromGroupChat(chatId , userId);
        return Ok();
    }
}