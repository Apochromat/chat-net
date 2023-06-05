using System.ComponentModel.DataAnnotations;
using ChatNet.Common.Enumerations;

namespace ChatNet.Common.DataTransferObjects; 

/// <summary>
/// Data transfer object for online preferences 
/// </summary>
public class OnlinePreferenceFullDto {
    /// <summary>
    /// Identifier of the user
    /// </summary>
    [Required]
    public required Guid UserId { get; set; }
    
    /// <summary>
    /// Type of the online preference
    /// </summary>
    [Required]
    public required OnlinePreferenceType Type { get; set; }
    
    /// <summary>
    /// Friends of the user
    /// </summary>
    [Required]
    public required List<Guid> Friends { get; set; } = new();
}