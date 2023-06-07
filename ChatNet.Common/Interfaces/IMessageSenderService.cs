using ChatNet.Common.DataTransferObjects;

namespace ChatNet.Common.Interfaces; 

/// <summary>
/// Message sender service
/// </summary>
public interface IMessageSenderService {
    /// <summary>
    /// send notification to rabbitMq
    /// </summary>
    /// <param name="messageDto"></param>
    /// <returns></returns>
    public Task SendNotification(MessageAuthToBackendDto messageDto);
}