using System.ComponentModel.DataAnnotations;
using ChatNet.Common.Enumerations;

namespace ChatNet.Common.DataTransferObjects; 

/// <summary>
/// DTO fo file editing
/// </summary>
public class FileEditDto {
    /// <summary>
    /// New name of the file
    /// </summary>
    [Required]
    public required string Name { get; set; }
    
    /// <summary>
    /// New content type of the file in MIME format
    /// </summary>
    [Required]
    public required FileType Type { get; set; }
}