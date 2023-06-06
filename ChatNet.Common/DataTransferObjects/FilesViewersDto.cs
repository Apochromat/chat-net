using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;

namespace ChatNet.Common.DataTransferObjects; 

/// <summary>
/// DTO for setting, adding and removing viewers of files.
/// </summary>
public class FilesViewersDto {
    /// <summary>
    /// List of files.
    /// </summary>
    [Required]
    [JsonPropertyName("files")]
    public required List<Guid> Files { get; set; } = new();
    
    /// <summary>
    /// List of viewers.
    /// </summary>
    [Required]
    [JsonPropertyName("viewers")]
    public required List<Guid> Viewers { get; set; } = new();
}