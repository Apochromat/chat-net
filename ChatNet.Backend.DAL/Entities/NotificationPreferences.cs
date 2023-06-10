using ChatNet.Common.Enumerations;

namespace ChatNet.Backend.DAL.Entities; 

public class NotificationPreferences {
    public Guid Id { get; set; } = Guid.NewGuid();
    public Chat PreferencedChat { get; set; }
    public UserBackend User { get; set; } 
    public NotificationPreferenceType PreferenceType { get; set; }
}