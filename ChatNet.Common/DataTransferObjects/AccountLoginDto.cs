using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

namespace ChatNet.Common.DataTransferObjects; 


/// <summary>
/// Dto for login
/// </summary>
public class AccountLoginDto {
    /// <summary>
    /// User email
    /// </summary>
    [Required]
    [EmailAddress]
    [DisplayName("email")]
    public required string Email { get; set; }

    /// <summary>
    /// User password
    /// </summary>
    [Required]
    [DefaultValue("P@ssw0rd")]
    [DisplayName("password")]
    public required string Password { get; set; }
}