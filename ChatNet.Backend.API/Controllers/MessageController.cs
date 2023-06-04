using ChatNet.Common.DataTransferObjects;
using ChatNet.Common.Exceptions;
using ChatNet.Common.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace ChatNet.Backend.API.Controllers; 

[ApiController]
[Route("api/backend/message")]
public class MessageController: ControllerBase {
    private readonly ILogger<MessageController> _logger;
    private readonly IMessageService _messageService;
    private readonly IPermissionCheckService _permissionCheckService;
    
    public MessageController(ILogger<MessageController> logger, 
        IMessageService messageService,
        IPermissionCheckService permissionCheckService) {
        _logger = logger;
        _messageService = messageService;
        _permissionCheckService = permissionCheckService;
    }

    /// <summary>
    /// Send message to chat
    /// </summary>
    /// <param name="model"></param>
    /// <param name="chatId"></param>
    /// <returns></returns>
    /// <exception cref="UnauthorizedException"></exception>
    [HttpPost]
    [Route("/chat/{chatId}")]
    [Authorize(AuthenticationSchemes = "Bearer")]
    public async Task<ActionResult> SendMessage([FromBody] MessageActionsDto model, Guid chatId) {
        if (User.Identity == null || Guid.TryParse(User.Identity.Name, out Guid userId) == false) {
            throw new UnauthorizedException("User is not authorized");
        }
        await _permissionCheckService.CheckUserHasAccessToChat(userId, chatId);
        await _messageService.SendMessage(model , userId, chatId);
        return Ok();
    }
    
    /// <summary>
    /// Edit message 
    /// </summary>
    /// <param name="model"></param>
    /// <param name="messageId"></param>
    /// <returns></returns>
    /// <exception cref="UnauthorizedException"></exception>
    [HttpPut]
    [Route("{messageId}")]
    [Authorize(AuthenticationSchemes = "Bearer")]
    public async Task<ActionResult> EditMessage([FromBody] MessageActionsDto model, Guid messageId) {
        if (User.Identity == null || Guid.TryParse(User.Identity.Name, out Guid userId) == false) {
            throw new UnauthorizedException("User is not authorized");
        }
        await _messageService.EditMessage(model , messageId, userId);
        return Ok();
    }
    
    /// <summary>
    /// Delete message
    /// </summary>
    /// <param name="messageId"></param>
    /// <returns></returns>
    /// <exception cref="UnauthorizedException"></exception>
    [HttpDelete]
    [Route("{messageId}")]
    [Authorize(AuthenticationSchemes = "Bearer")]
    public async Task<ActionResult> DeleteMessage(Guid messageId) {
        if (User.Identity == null || Guid.TryParse(User.Identity.Name, out Guid userId) == false) {
            throw new UnauthorizedException("User is not authorized");
        }
        await _messageService.DeleteMessage(messageId, userId);
        return Ok();
    }
}