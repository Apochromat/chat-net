using Microsoft.AspNetCore.Mvc;

namespace ChatNet.FileStorage.API.Controllers;

[ApiController]
[Route("[controller]")]
public class FileStorageController : ControllerBase {
    private readonly ILogger<FileStorageController> _logger;

    public FileStorageController(ILogger<FileStorageController> logger) {
        _logger = logger;
    }

    [HttpPost]
    public ActionResult UploadFile() {
        return Ok();
    }
}