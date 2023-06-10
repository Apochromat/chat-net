namespace ChatNet.Backend.DAL.Entities; 

public class Chat {
    public Guid Id { get; set; } = Guid.NewGuid();
    public Guid? ChatAvatarId { get; set; } 
    public string ChatName { get; set; }
    public List<Message> Messages { get; set; } = new List<Message>();
    public List<Guid> FileIds { get; set; } = new List<Guid>();
    public List<UserBackend> Users { get; set; } = new List<UserBackend>();
    public DateTime CreatedTime { get; set; } = DateTime.UtcNow;
    public DateTime? DeletedTime { get; set; }
}