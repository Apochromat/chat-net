using System.ComponentModel.DataAnnotations;
using ChatNet.Common.Enumerations;

namespace ChatNet.Common.DataTransferObjects;

/// <summary>
/// Data transfer object for online preferences (for endpoint)
/// </summary>
public class OnlinePreferenceTypeDto {
    /// <summary>
    /// Type of the online preference
    /// </summary>
    [Required]
    public required OnlinePreferenceType Type { get; set; }
}