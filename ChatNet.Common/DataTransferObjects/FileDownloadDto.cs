using ChatNet.Common.Enumerations;
using Microsoft.AspNetCore.Http;

namespace ChatNet.Common.DataTransferObjects; 

/// <summary>
/// DTO for file download
/// </summary>
public class FileDownloadDto {
    /// <summary>
    /// File identifier
    /// </summary>
    public Guid Id { get; set; }
    
    /// <summary>
    /// Name of the file
    /// </summary>
    public required string Name { get; set; }
    
    /// <summary>
    /// Content type of the file in MIME format
    /// </summary>
    public required string ContentType { get; set; }

    /// <summary>
    /// File content
    /// </summary>
    public required byte[] Content { get; set; }
}