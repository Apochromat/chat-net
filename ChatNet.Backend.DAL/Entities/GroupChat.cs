namespace ChatNet.Backend.DAL.Entities; 

public class GroupChat: Chat {
    public List<UserBackend> Administrators { get; set; } = new List<UserBackend>();
}