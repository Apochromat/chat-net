using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

namespace ChatNet.Common.DataTransferObjects; 

/// <summary>
/// Data transfer object for changing password
/// </summary>
public class ChangePasswordDto {
    /// <summary>
    /// Old password
    /// </summary>
    [Required]
    [DisplayName("old_password")]
    public required string OldPassword { get; set; }

    /// <summary>
    /// New password
    /// </summary>
    [Required]
    [DisplayName("new_password")]
    public required string NewPassword { get; set; }
}