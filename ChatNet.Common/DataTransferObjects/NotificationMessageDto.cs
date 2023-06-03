using System.ComponentModel;

namespace ChatNet.Common.DataTransferObjects; 

public class NotificationMessageDto {
    public string Title { get; set; }
    public Guid NewUserId { get; set; }
}