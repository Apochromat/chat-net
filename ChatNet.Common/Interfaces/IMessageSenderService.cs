using ChatNet.Common.DataTransferObjects;

namespace ChatNet.Common.Interfaces; 

public interface IMessageSenderService {
    public Task SendNotification(NotificationMessageDto messageDto);
}