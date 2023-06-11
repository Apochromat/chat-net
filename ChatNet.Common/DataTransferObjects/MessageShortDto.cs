namespace ChatNet.Common.DataTransferObjects; 

public class MessageShortDto {
    public Guid Id { get; set; }
    public Guid SenderId { get; set; }
    public string TextMessage { get; set; }
    public DateTime CreatedTime { get; set; }
}