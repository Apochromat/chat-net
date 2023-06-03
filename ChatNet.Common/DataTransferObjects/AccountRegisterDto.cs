using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

namespace ChatNet.Common.DataTransferObjects; 


/// <summary>
/// DTO for user registration
/// </summary>
public class AccountRegisterDto {
    /// <summary>
    /// User`s email
    /// </summary>
    [Required]
    [EmailAddress]
    [DisplayName("email")]
    public required string Email { get; set; }

    /// <summary>
    /// User`s password
    /// </summary>
    [Required]
    [DefaultValue("P@ssw0rd")]
    [DisplayName("password")]
    [MinLength(8)]
    public required string Password { get; set; }

    /// <summary>
    /// User`s full name (surname, name, patronymic)
    /// </summary>
    [Required]
    public required string FullName { get; set; }

    /// <summary>
    /// User`s phone number
    /// </summary>
    [Required]
    [Phone]
    public required string PhoneNumber { get; set; }

    /// <summary>
    /// User`s birth date
    /// </summary>
    [Required]
    [Range(typeof(DateTime), "01/01/1900", "01/01/2023")]
    public required DateTime BirthDate { get; set; }
}