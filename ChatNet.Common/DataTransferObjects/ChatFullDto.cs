namespace ChatNet.Common.DataTransferObjects; 

public class ChatFullDto {
    public Guid Id { get; set; }
    public string ChatName { get; set; }
    public Guid? ChatAvatarId { get; set; }
    public DateTime CreatedTime { get; set; }
    public DateTime? DeletedTime { get; set; }
    public List<Guid> FileIds { get; set; } = new List<Guid>();
    public List<Guid> Users { get; set; } = new List<Guid>();
}