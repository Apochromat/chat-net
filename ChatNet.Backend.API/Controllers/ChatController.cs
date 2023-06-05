using ChatNet.Common.DataTransferObjects;
using ChatNet.Common.Enumerations;
using ChatNet.Common.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace ChatNet.Backend.API.Controllers;

[ApiController]
[Route("api/backend")]
public class ChatController : ControllerBase {
    private readonly ILogger<ChatController> _logger;
    private readonly INotificationQueueService _notificationQueueService;

    public ChatController(ILogger<ChatController> logger, INotificationQueueService notificationQueueService) {
        _logger = logger;
        _notificationQueueService = notificationQueueService;
    }

    [HttpGet]
    public async Task<ActionResult> GetUserChats() {
        await _notificationQueueService.SendNotificationAsync(new NotificationMessageDto {
            Type = NotificationMessageType.NewMessage,
            Title = "Hello",
            Text = "Hello World",
            ReceiverId = Guid.Parse("c49caed6-d1af-4e51-b8c3-1be971638d41"),
            SenderId = Guid.NewGuid()
        });
        return Ok();
    }
}