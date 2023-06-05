using System.Text;
using System.Text.Json;
using ChatNet.Common.DataTransferObjects;
using ChatNet.Common.Interfaces;
using Microsoft.Extensions.Configuration;
using RabbitMQ.Client;

namespace ChatNet.Backend.BLL.Services; 

/// <inheritdoc cref="INotificationQueueService"/>
public class NotificationQueueService : INotificationQueueService {
    private readonly IConfiguration _configuration;

    /// <summary>
    /// Constructor
    /// </summary>
    /// <param name="configuration"></param>
    public NotificationQueueService(IConfiguration configuration) {
        _configuration = configuration;
    }
    
    public Task SendNotificationAsync(NotificationMessageDto notificationMessageDto) {
        var factory = new ConnectionFactory() { HostName = _configuration["RabbitMQ:HostName"] };
        try {
            using var connection = factory.CreateConnection();
            using var channel = connection.CreateModel();
            channel.ExchangeDeclare(exchange: _configuration["RabbitMQ:NotificationsExchangeName"], type: ExchangeType.Fanout);
                
            var message = JsonSerializer.Serialize(notificationMessageDto);
            var body = Encoding.UTF8.GetBytes(message);

            channel.BasicPublish(exchange: _configuration["RabbitMQ:NotificationsExchangeName"],
                routingKey: "",
                basicProperties: null,
                body: body);
        }
        catch (Exception) {
            // ignored
        }
        return Task.CompletedTask;
    }

    public Task SendOnlinePreferenceAsync(Guid userId, OnlinePreferenceDto onlinePreferenceDto) {
        var factory = new ConnectionFactory() { HostName = _configuration["RabbitMQ:HostName"] };
        try {
            using var connection = factory.CreateConnection();
            using var channel = connection.CreateModel();
            channel.ExchangeDeclare(exchange: _configuration["RabbitMQ:PreferencesExchangeName"], type: ExchangeType.Fanout);
                
            var message = JsonSerializer.Serialize(onlinePreferenceDto);
            var body = Encoding.UTF8.GetBytes(message);

            channel.BasicPublish(exchange: _configuration["RabbitMQ:PreferencesExchangeName"],
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