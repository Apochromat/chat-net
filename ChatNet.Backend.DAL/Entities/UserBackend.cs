using System.ComponentModel.DataAnnotations.Schema;

namespace ChatNet.Backend.DAL.Entities; 

public class UserBackend {
    public Guid Id { get; set; }
    public List<Chat> Chats { get; set; } = new();
    [InverseProperty("Friends")]
    public virtual ICollection<UserBackend> Users { get; set; }
    public virtual ICollection<UserBackend> Friends { get; set; }
    public List<NotificationPreferences> ChatsNotificationPreferences { get; set; } = new();
    public List<Reaction> Reactions { get; set; } = new();
}