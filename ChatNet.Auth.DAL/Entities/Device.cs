namespace ChatNet.Auth.DAL.Entities; 

/// <summary>
/// Entity for user devices, refresh tokens, etc.
/// </summary>
public class Device {
    /// <summary>
    /// Device identifier
    /// </summary>
    public Guid Id { get; set; }

    /// <summary>
    /// User identifier
    /// </summary>
    public required User User { get; set; }

    /// <summary>
    /// User agent
    /// </summary>
    public required string UserAgent { get; set; }
    
    /// <summary>
    /// User agent
    /// </summary>
    public required string IpAddress { get; set; }

    /// <summary>
    /// Device name
    /// </summary>
    public string? DeviceName { get; set; }

    /// <summary>
    /// Refresh token
    /// </summary>
    public required string RefreshToken { get; set; }

    /// <summary>
    /// Last activity
    /// </summary>
    public DateTime LastActivity { get; set; }

    /// <summary>
    /// Date of creation
    /// </summary>
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

    /// <summary>
    /// Expiration date
    /// </summary>
    public DateTime ExpirationDate { get; set; }
}