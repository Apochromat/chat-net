namespace ChatNet.Backend.DAL.Entities; 

public class Message {
    public Guid Id { get; set; } = Guid.NewGuid();
    public UserBackend User { get; set; }
    public string TextMessage { get; set; }
    public DateTime CreatedTime { get; set; } = DateTime.UtcNow;
    public DateTime? DeletedTime { get; set; }
    public DateTime? EditedTime { get; set; }
    public List<Reaction> Reactions { get; set; } = new List<Reaction>();
    public List<Guid> Files { get; set; } = new List<Guid>();
    public List<Guid> ViewedBy { get; set; } = new List<Guid>();
}