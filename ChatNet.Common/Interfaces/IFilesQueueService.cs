using ChatNet.Common.DataTransferObjects;

namespace ChatNet.Common.Interfaces; 

/// <summary>
/// Files queue service
/// </summary>
public interface IFilesQueueService {
    /// <summary>
    /// Set files viewers in notification component
    /// </summary>
    /// <param name="filesViewersDto"></param>
    /// <returns></returns>
    Task SetViewersAsync(FilesViewersDto filesViewersDto);
}