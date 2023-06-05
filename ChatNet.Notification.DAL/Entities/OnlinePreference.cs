using ChatNet.Common.Enumerations;

namespace ChatNet.Notification.DAL.Entities; 

/// <summary>
/// Online preference entity
/// </summary>
public class OnlinePreference {
    /// <summary>
    /// Entity identifier
    /// </summary>
    public Guid Id { get; set; }
    
    /// <summary>
    /// User identifier
    /// </summary>
    public required Guid UserId { get; set; }
    
    /// <summary>
    /// Online preference type
    /// </summary>
    public required OnlinePreferenceType Type { get; set; }
    
    /// <summary>
    /// Friends
    /// </summary>
    public List<Guid> Friends { get; set; } = new();
}