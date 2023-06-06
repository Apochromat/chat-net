namespace ChatNet.Backend.DAL.Entities; 

public class UserBackend {
    public Guid Id { get; set; }
    public List<Chat> Chats { get; set; } = new List<Chat>();
    public List<UserBackend> Friends { get; set; } = new List<UserBackend>();
    public List<NotificationPreferences> ChatsNotificationPreferences { get; set; } =
        new List<NotificationPreferences>();
    public List<Reaction> Reactions { get; set; } = new List<Reaction>();
}