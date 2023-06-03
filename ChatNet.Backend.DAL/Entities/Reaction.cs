using ChatNet.Common.Enumerations;

namespace ChatNet.Backend.DAL.Entities; 

public class Reaction {
    public Guid Id { get; set; } = Guid.NewGuid();
    public List<UserBackend> Users  { get; set; } = new List<UserBackend>();
    public ReactionType ReactionType { get; set; } 
    public Message ReactedMessage { get; set; }
}