using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;
using ChatNet.Common.Enumerations;
using Microsoft.AspNetCore.Http;

namespace ChatNet.Common.DataTransferObjects; 

/// <summary>
/// DTO that represents a file upload
/// </summary>
public class FileUploadDto {
    /// <summary>
    /// Name of the file
    /// </summary>
    [Required]
    [JsonPropertyName("name")]
    [RegularExpression(@"[a-zA-Z0-9\-_\.]+", ErrorMessage = "Invalid file name")]
    public required string Name { get; set; }
    
    /// <summary>
    /// Content type of the file in MIME format
    /// </summary>
    [Required]
    [JsonPropertyName("contentType")]
    [RegularExpression(@"[a-z]+/[a-z]+", ErrorMessage = "Invalid content type")]
    public required string ContentType { get; set; }
    
    /// <summary>
    /// File type
    /// </summary>
    [Required]
    [JsonPropertyName("fileType")]
    public required FileType Type { get; set; }
    
    /// <summary>
    /// Is the file public
    /// </summary>
    public required bool IsPublic { get; set; }
    
    /// <summary>
    /// Identifier of the user who uploaded the file
    /// </summary>
    [Required]
    [JsonPropertyName("ownerId")]
    public required Guid OwnerId { get; set; }
    
    /// <summary>
    /// List of users who can view the file
    /// </summary>
    [Required]
    [JsonPropertyName("viewers")]
    public required List<Guid> Viewers { get; set; } = new();

    /// <summary>
    /// File content
    /// </summary>
    [Required]
    [JsonPropertyName("content")]
    public required IFormFile Content { get; set; }
}