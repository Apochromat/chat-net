using System.Text;
using System.Text.Json;
using ChatNet.Common.DataTransferObjects;
using ChatNet.Common.Interfaces;
using Microsoft.Extensions.Configuration;
using RabbitMQ.Client;

namespace ChatNet.Auth.BLL.Services; 

public class MessageSernderService : IMessageSenderService {
    private readonly IConfiguration _configuration;

    /// <summary>
    /// Constructor
    /// </summary>
    /// <param name="configuration"></param>
    public MessageSernderService(IConfiguration configuration) {
        _configuration = configuration;
    }

    /// <summary>
    /// Sends notification to user
    /// </summary>
    /// <param name="messageDto"></param>
    public Task SendNotification(NotificationMessageDto messageDto) {
        var factory = new ConnectionFactory() { HostName = _configuration["RabbitMQ:HostName"] };
        try {
            using var connection = factory.CreateConnection();
            using var channel = connection.CreateModel();
            channel.ExchangeDeclare(exchange: _configuration["RabbitMQ:ExchangeName"], type: ExchangeType.Fanout);
                
            var message = JsonSerializer.Serialize(messageDto);
            var body = Encoding.UTF8.GetBytes(message);

            channel.BasicPublish(exchange: _configuration["RabbitMQ:ExchangeName"],
                routingKey: "",
                basicProperties: null,
                body: body);
        }
        catch (Exception) {
            // ignored
        }
        return Task.CompletedTask;
    }
}