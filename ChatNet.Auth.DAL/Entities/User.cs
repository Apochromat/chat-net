using Microsoft.AspNetCore.Identity;

namespace ChatNet.Auth.DAL.Entities; 
/// <summary>
/// AuthDB general User Model   
/// </summary>
public class User : IdentityUser<Guid> {
    /// <summary>
    /// User`s full name (surname, name, patronymic)
    /// </summary>
    public required string FullName { get; set; }

    /// <summary>
    /// User`s birth date
    /// </summary>
    public DateTime BirthDate { get; set; }

    /// <summary>
    /// Date when user joined the system
    /// </summary>
    public DateTime JoinedAt { get; set; } = DateTime.UtcNow;

    /// <summary>
    /// User`s devices
    /// </summary>
    public List<Device> Devices { get; set; } = new List<Device>();
}