namespace ChatNet.Common.DataTransferObjects; 

public class ChatFullDto {
    public Guid Id { get; set; }
    public string ChatName { get; set; }
    public Guid ChatAvatarId { get; set; }
    public List<MessageDto> Messages { get; set; }
    public List<Guid> Users { get; set; }
}