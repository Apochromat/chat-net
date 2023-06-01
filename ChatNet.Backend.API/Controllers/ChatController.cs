using Microsoft.AspNetCore.Mvc;

namespace ChatNet.Backend.API.Controllers;

[ApiController]
[Route("[controller]")]
public class ChatController : ControllerBase {
    private readonly ILogger<ChatController> _logger;

    public ChatController(ILogger<ChatController> logger) {
        _logger = logger;
    }

    [HttpGet]
    public ActionResult GetUserChats() {
        return Ok();
    }
}