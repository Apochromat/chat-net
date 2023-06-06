using ChatNet.Common.DataTransferObjects;
using ChatNet.Common.Enumerations;

namespace ChatNet.Common.Interfaces; 

/// <summary>
/// Interface for file storage service
/// </summary>
public interface IFileStorageService {
    /// <summary>
    /// Uploads file to the storage
    /// </summary>
    /// <param name="fileUploadDto"></param>
    /// <returns></returns>
    Task<Guid> UploadFileAsync(FileUploadDto fileUploadDto);

    /// <summary>
    /// Gets information about all files which are owned by user
    /// </summary>
    /// <param name="ownerId"></param>
    /// <param name="page"></param>
    /// <param name="pageSize"></param>
    /// <param name="fileTypes"></param>
    /// <returns></returns>
    Task<Pagination<FileInfoDto>> GetOwnedFilesInfoAsync(Guid ownerId, int page, int pageSize, List<FileType>? fileTypes = null);

    /// <summary>
    /// Gets information about all files which are shared with user
    /// </summary>
    /// <param name="userId"></param>
    /// <param name="page"></param>
    /// <param name="pageSize"></param>
    /// <param name="fileTypes"></param>
    /// <returns></returns>
    Task<Pagination<FileInfoDto>> GetSharedWithUserFilesInfoAsync(Guid userId, int page, int pageSize, List<FileType>? fileTypes = null);

    /// <summary>
    /// Gets information about file
    /// </summary>
    /// <param name="fileId"></param>
    /// <param name="userId"></param>
    /// <returns></returns>
    Task<FileInfoDto> GetFileInfoAsync(Guid fileId, Guid userId);

    /// <summary>
    /// Downloads file from the storage
    /// </summary>
    /// <param name="fileId"></param>
    /// <param name="userId"></param>
    /// <returns></returns>
    Task<FileDownloadDto> DownloadFileAsync(Guid fileId, Guid userId);
    
    /// <summary>
    /// Edits file in the storage
    /// </summary>
    /// <param name="fileId"></param>
    /// <param name="userId"></param>
    /// <param name="fileEditDto"></param>
    /// <returns></returns>
    Task EditFileAsync(Guid fileId, Guid userId, FileEditDto fileEditDto);

    /// <summary>
    /// Deletes file from the storage
    /// </summary>
    /// <param name="fileId"></param>
    /// <param name="userId"></param>
    /// <returns></returns>
    Task DeleteFileAsync(Guid fileId, Guid userId);
    
    /// <summary>
    /// Checks if user is the owner of the file
    /// </summary>
    /// <param name="fileId"></param>
    /// <param name="userId"></param>
    /// <returns></returns>
    Task<bool> IsFileOwnerAsync(Guid fileId, Guid userId);

    /// <summary>
    /// Checks if user is able to view the file
    /// </summary>
    /// <param name="fileId"></param>
    /// <param name="userId"></param>
    /// <returns></returns>
    Task<bool> IsFileOwnerOrViewerAsync(Guid fileId, Guid userId);

    /// <summary>
    /// Adds users to the lists of viewers of the files
    /// </summary>
    /// <param name="filesViewersDto"></param>
    /// <param name="ownerId"></param>
    /// <param name="raiseExceptionIfAlreadyExists"></param>
    /// <param name="checkOwner"></param>
    /// <returns></returns>
    Task AddViewerAsync(FilesViewersDto filesViewersDto, Guid? ownerId = null,
        bool raiseExceptionIfAlreadyExists = false, bool checkOwner = true);

    /// <summary>
    /// Removes users from the lists of viewers of the files
    /// </summary>
    /// <param name="filesViewersDto"></param>
    /// <param name="ownerId"></param>
    /// <param name="raiseExceptionIfNotExists"></param>
    /// <param name="checkOwner"></param>
    /// <returns></returns>
    Task RemoveViewerAsync(FilesViewersDto filesViewersDto, Guid? ownerId = null,
        bool raiseExceptionIfNotExists = false, bool checkOwner = true);

    /// <summary>
    /// Sets viewers of the files
    /// </summary>
    /// <param name="filesViewersDto"></param>
    /// <param name="ownerId"></param>
    /// <param name="checkOwner"></param>
    /// <returns></returns>
    Task SetViewerAsync(FilesViewersDto filesViewersDto, Guid? ownerId = null, bool checkOwner = true);
}