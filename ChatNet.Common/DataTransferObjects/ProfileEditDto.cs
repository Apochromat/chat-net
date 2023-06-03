using System.ComponentModel.DataAnnotations;

namespace ChatNet.Common.DataTransferObjects; 
/// <summary>
/// Profile DTO for Edit
/// </summary>
public class ProfileEditDto {
    
    
    /// <summary>
    /// User`s full name (surname, name, patronymic)
    /// </summary>
    [Required]
    public required string FullName { get; set; }

    /// <summary>
    /// User`s birth date
    /// </summary>
    [Required]
    [Range(typeof(DateTime), "01/01/1900", "01/01/2023")]
    public required DateTime BirthDate { get; set; }

}