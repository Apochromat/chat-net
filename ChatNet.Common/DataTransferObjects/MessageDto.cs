namespace ChatNet.Common.DataTransferObjects; 

public class MessageDto {
    public Guid Id { get; set; }
    public Guid SenderId { get; set; }
    public string TextMessage { get; set; }
    public DateTime CreatedTime { get; set; }
    public DateTime? EditedTime { get; set; }
    public List<Guid> FileIds { get; set; } = new List<Guid>();
    public List<ReactionDto> MessageReactions { get; set; } = new List<ReactionDto>();
    public bool IsViewed { get; set; }

}