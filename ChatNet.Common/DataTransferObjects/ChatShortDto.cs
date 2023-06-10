namespace ChatNet.Common.DataTransferObjects; 

public class ChatShortDto {
    public Guid Id { get; set; }
    public Guid? ChatAvatarId { get; set; }
    public string ChatName { get; set; }
    public DateTime? DeletedTime { get; set; }
}