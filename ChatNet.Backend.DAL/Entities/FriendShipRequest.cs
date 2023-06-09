namespace ChatNet.Backend.DAL.Entities; 

public class FriendShipRequest {
    public Guid Id { get; set; } = Guid.NewGuid();
    public UserBackend RequestFrom { get; set; }
    public UserBackend RequestTo { get; set; }
}