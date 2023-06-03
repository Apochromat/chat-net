using System.ComponentModel.DataAnnotations;
using System.Runtime.InteropServices;
using ChatNet.Common.DataTransferObjects;
using ChatNet.Common.Enumerations;
using ChatNet.Common.Exceptions;
using ChatNet.Common.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace ChatNet.FileStorage.API.Controllers;

/// <summary>
/// Controller for file storage
/// </summary>
[ApiController]
[Authorize(AuthenticationSchemes = "Bearer")]
[Route("api/files")]
public class FileStorageController : ControllerBase {
    private readonly ILogger<FileStorageController> _logger;
    private readonly IFileStorageService _fileStorageService;

    /// <summary>
    /// Constructor
    /// </summary>
    /// <param name="logger"></param>
    /// <param name="fileStorageService"></param>
    public FileStorageController(ILogger<FileStorageController> logger, IFileStorageService fileStorageService) {
        _logger = logger;
        _fileStorageService = fileStorageService;
    }

    /// <summary>
    /// Upload file
    /// </summary>
    /// <returns></returns>
    [HttpPost]
    public async Task<ActionResult<Guid>> UploadFile(IFormFile formFile, [FromQuery] [Required] FileType fileType,
        [FromQuery] [Optional] List<Guid> viewers, [FromQuery] [Required] bool isPublic = false) {
        if (User.Identity == null || Guid.TryParse(User.Identity.Name, out Guid userId) == false) {
            throw new UnauthorizedException("User is not authorized");
        }

        var id = await _fileStorageService.UploadFileAsync(new FileUploadDto() {
            Content = formFile,
            ContentType = formFile.ContentType,
            Name = formFile.FileName,
            Type = fileType,
            IsPublic = isPublic,
            OwnerId = userId,
            Viewers = viewers
        });
        _logger.LogInformation("File was uploaded");
        return Created($"api/files/{id}", id);
    }

    /// <summary>
    /// Get owned files
    /// </summary>
    /// <param name="page"></param>
    /// <param name="pageSize"></param>
    /// <param name="fileTypes"></param>
    /// <returns></returns>
    [HttpGet]
    public async Task<ActionResult<Pagination<FileInfoDto>>> GetOwnedFilesInfo([FromQuery] int page = 1,
        [FromQuery] int pageSize = 15, [FromQuery] List<FileType>? fileTypes = null) {
        if (User.Identity == null || Guid.TryParse(User.Identity.Name, out Guid userId) == false) {
            throw new UnauthorizedException("User is not authorized");
        }

        var files = await _fileStorageService.GetOwnedFilesInfoAsync(userId, page, pageSize, fileTypes);
        _logger.LogInformation("Owned files info was retrieved");
        return Ok(files);
    }

    /// <summary>
    /// Get files shared with user
    /// </summary>
    /// <param name="page"></param>
    /// <param name="pageSize"></param>
    /// <param name="fileTypes"></param>
    /// <returns></returns>
    [HttpGet]
    [Route("shared")]
    public async Task<ActionResult<Pagination<FileInfoDto>>> GetSharedWithUserFilesInfo([FromQuery] int page = 1,
        [FromQuery] int pageSize = 15, [FromQuery] List<FileType>? fileTypes = null) {
        if (User.Identity == null || Guid.TryParse(User.Identity.Name, out Guid userId) == false) {
            throw new UnauthorizedException("User is not authorized");
        }

        var files = await _fileStorageService.GetSharedWithUserFilesInfoAsync(userId, page, pageSize, fileTypes);
        _logger.LogInformation("Shared files info was retrieved");
        return Ok(files);
    }

    /// <summary>
    /// Download file
    /// </summary>
    /// <returns></returns>
    [HttpGet]
    [Route("{fileId}")]
    public async Task<FileContentResult> DownloadFile([FromRoute] Guid fileId, bool attachment = false) {
        if (User.Identity == null || Guid.TryParse(User.Identity.Name, out Guid userId) == false) {
            throw new UnauthorizedException("User is not authorized");
        }

        var file = await _fileStorageService.DownloadFileAsync(fileId, userId);
        _logger.LogInformation("File was downloaded");
        var result = attachment
            ? File(file.Content, file.ContentType, file.Name)
            : File(file.Content, file.ContentType);
        result.EnableRangeProcessing = true;

        return result;
    }

    /// <summary>
    /// Get file info
    /// </summary>
    /// <param name="fileId"></param>
    /// <returns></returns>
    [HttpGet]
    [Route("{fileId}/info")]
    public async Task<ActionResult<FileInfoDto>> GetFileInfo([FromRoute] Guid fileId) {
        if (User.Identity == null || Guid.TryParse(User.Identity.Name, out Guid userId) == false) {
            throw new UnauthorizedException("User is not authorized");
        }

        var file = await _fileStorageService.GetFileInfoAsync(fileId, userId);
        _logger.LogInformation("File info was retrieved");
        return Ok(file);
    }

    /// <summary>
    /// Edit file
    /// </summary>
    /// <param name="fileId"></param>
    /// <param name="fileEditDto"></param>
    /// <returns></returns>
    [HttpPut]
    [Route("{fileId}")]
    public async Task<ActionResult> EditFile([FromRoute] Guid fileId, [FromBody] FileEditDto fileEditDto) {
        if (User.Identity == null || Guid.TryParse(User.Identity.Name, out Guid userId) == false) {
            throw new UnauthorizedException("User is not authorized");
        }

        await _fileStorageService.EditFileAsync(fileId, userId, fileEditDto);
        _logger.LogInformation("File was edited");
        return Ok();
    }

    /// <summary>
    /// Delete file
    /// </summary>
    /// <param name="fileId"></param>
    /// <returns></returns>
    [HttpDelete]
    [Route("{fileId}")]
    public async Task<ActionResult> DeleteFile([FromRoute] Guid fileId) {
        if (User.Identity == null || Guid.TryParse(User.Identity.Name, out Guid userId) == false) {
            throw new UnauthorizedException("User is not authorized");
        }

        await _fileStorageService.DeleteFileAsync(fileId, userId);
        _logger.LogInformation("File was deleted");
        return Ok();
    }

    /// <summary>
    /// Check if user is owner of the file
    /// </summary>
    /// <param name="fileId"></param>
    /// <returns></returns>
    [HttpGet]
    [Route("{fileId}/is-owner")]
    public async Task<ActionResult<bool>> IsOwner([FromRoute] Guid fileId) {
        if (User.Identity == null || Guid.TryParse(User.Identity.Name, out Guid userId) == false) {
            throw new UnauthorizedException("User is not authorized");
        }

        var isOwner = await _fileStorageService.IsFileOwnerAsync(fileId, userId);
        _logger.LogInformation("Is owner was retrieved");
        return Ok(isOwner);
    }

    /// <summary>
    /// Check if user is able to view the file
    /// </summary>
    /// <param name="fileId"></param>
    /// <returns></returns>
    [HttpGet]
    [Route("{fileId}/is-viewer")]
    public async Task<ActionResult<bool>> IsViewer([FromRoute] Guid fileId) {
        if (User.Identity == null || Guid.TryParse(User.Identity.Name, out Guid userId) == false) {
            throw new UnauthorizedException("User is not authorized");
        }

        var isViewer = await _fileStorageService.IsFileOwnerOrViewerAsync(fileId, userId);
        _logger.LogInformation("Is viewer was retrieved");
        return Ok(isViewer);
    }

    /// <summary>
    /// Add viewer to file
    /// </summary>
    /// <param name="fileId"></param>
    /// <param name="userId"></param>
    /// <returns></returns>
    [HttpPost]
    [Route("{fileId}/add-viewer")]
    public async Task<ActionResult> AddViewerToFile([FromRoute] Guid fileId, [FromQuery] [Required] Guid userId) {
        if (User.Identity == null || Guid.TryParse(User.Identity.Name, out Guid ownerId) == false) {
            throw new UnauthorizedException("User is not authorized");
        }

        await _fileStorageService.AddViewerToFileAsync(fileId, userId, ownerId);
        _logger.LogInformation("Viewer of file was added");
        return Ok();
    }

    /// <summary>
    /// Remove viewer from file
    /// </summary>
    /// <param name="fileId"></param>
    /// <param name="userId"></param>
    /// <returns></returns>
    [HttpDelete]
    [Route("{fileId}/remove-viewer")]
    public async Task<ActionResult> RemoveViewerFromFile([FromRoute] Guid fileId, [FromQuery] [Required] Guid userId) {
        if (User.Identity == null || Guid.TryParse(User.Identity.Name, out Guid ownerId) == false) {
            throw new UnauthorizedException("User is not authorized");
        }

        await _fileStorageService.RemoveViewerFromFileAsync(fileId, userId, ownerId);
        _logger.LogInformation("Viewer of file was removed");
        return Ok();
    }

    /// <summary>
    /// Add viewer to files
    /// </summary>
    /// <param name="filesId"></param>
    /// <param name="userId"></param>
    /// <returns></returns>
    [HttpPost]
    [Route("add-viewer")]
    public async Task<ActionResult> AddViewerToFiles([FromQuery] [Required] List<Guid> filesId,
        [FromQuery] [Required] Guid userId) {
        if (User.Identity == null || Guid.TryParse(User.Identity.Name, out Guid ownerId) == false) {
            throw new UnauthorizedException("User is not authorized");
        }

        await _fileStorageService.AddViewerToFilesAsync(filesId, userId, ownerId);
        _logger.LogInformation("Viewer were added to files");
        return Ok();
    }

    /// <summary>
    /// Remove viewer from files
    /// </summary>
    /// <param name="filesId"></param>
    /// <param name="userId"></param>
    /// <returns></returns>
    [HttpDelete]
    [Route("remove-viewer")]
    public async Task<ActionResult> RemoveViewerFromFiles([FromQuery] [Required] List<Guid> filesId,
        [FromQuery] [Required] Guid userId) {
        if (User.Identity == null || Guid.TryParse(User.Identity.Name, out Guid ownerId) == false) {
            throw new UnauthorizedException("User is not authorized");
        }

        await _fileStorageService.RemoveViewerFromFilesAsync(filesId, userId, ownerId);
        _logger.LogInformation("Viewer was removed from files");
        return Ok();
    }

    /// <summary>
    /// Add viewers to file
    /// </summary>
    /// <param name="fileId"></param>
    /// <param name="viewers"></param>
    /// <returns></returns>
    [HttpPost]
    [Route("{fileId}/add-viewers")]
    public async Task<ActionResult> AddViewersToFiles([FromRoute] Guid fileId,
        [FromQuery] [Required] List<Guid> viewers) {
        if (User.Identity == null || Guid.TryParse(User.Identity.Name, out Guid ownerId) == false) {
            throw new UnauthorizedException("User is not authorized");
        }

        await _fileStorageService.AddViewersToFileAsync(fileId, viewers, ownerId);
        _logger.LogInformation("Viewers of file were added");
        return Ok();
    }

    /// <summary>
    /// Remove viewers from file
    /// </summary>
    /// <param name="fileId"></param>
    /// <param name="viewers"></param>
    /// <returns></returns>
    [HttpDelete]
    [Route("{fileId}/remove-viewers")]
    public async Task<ActionResult> RemoveViewersFromFiles([FromRoute] Guid fileId,
        [FromQuery] [Required] List<Guid> viewers) {
        if (User.Identity == null || Guid.TryParse(User.Identity.Name, out Guid ownerId) == false) {
            throw new UnauthorizedException("User is not authorized");
        }

        await _fileStorageService.RemoveViewersFromFileAsync(fileId, viewers, ownerId);
        _logger.LogInformation("Viewers of file were removed");
        return Ok();
    }
}