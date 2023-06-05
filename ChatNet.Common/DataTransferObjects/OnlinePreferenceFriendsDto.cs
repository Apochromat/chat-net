using System.ComponentModel.DataAnnotations;
using ChatNet.Common.Enumerations;

namespace ChatNet.Common.DataTransferObjects; 

/// <summary>
/// Data transfer object for online preferences (for message queue)
/// </summary>
public class OnlinePreferenceFriendsDto {
    /// <summary>
    /// Identifier of the user
    /// </summary>
    [Required]
    public required Guid UserId { get; set; }
    
    /// <summary>
    /// Friends of the user
    /// </summary>
    [Required]
    public required List<Guid> Friends { get; set; } = new();
}