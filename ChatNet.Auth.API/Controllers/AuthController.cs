using Microsoft.AspNetCore.Mvc;

namespace ChatNet.Auth.API.Controllers;

[ApiController]
[Route("[controller]")]
public class AuthController : ControllerBase {
    private readonly ILogger<AuthController> _logger;

    public AuthController(ILogger<AuthController> logger) {
        _logger = logger;
    }

    [HttpPost]
    public ActionResult Login() {
        return Ok();
    }
}