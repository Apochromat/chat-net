using System.Text;
using System.Text.Json;
using ChatNet.Common.DataTransferObjects;
using ChatNet.Common.Interfaces;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using RabbitMQ.Client;

namespace ChatNet.Backend.BLL.Services; 

/// <inheritdoc cref="INotificationQueueService"/>
public class NotificationQueueService : INotificationQueueService {
    private readonly ILogger<NotificationQueueService> _logger;
    private readonly IConfiguration _configuration;

    /// <summary>
    /// Constructor
    /// </summary>
    /// <param name="configuration"></param>
    /// <param name="logger"></param>
    public NotificationQueueService(IConfiguration configuration, ILogger<NotificationQueueService> logger) {
        _configuration = configuration;
        _logger = logger;
    }
    
    public Task SendNotificationAsync(NotificationMessageDto notificationMessageDto) {
        var factory = new ConnectionFactory() { HostName = _configuration["RabbitMQ:Hostname"] };
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
        catch (Exception e) {
            _logger.LogError("Error while sending notification to RabbitMQ: {Message}", e.Message);
        }
        return Task.CompletedTask;
    }

    public Task SendOnlinePreferenceAsync(OnlinePreferenceFriendsDto onlinePreferenceFriendsDto) {
        var factory = new ConnectionFactory() { HostName = _configuration["RabbitMQ:Hostname"] };
        try {
            using var connection = factory.CreateConnection();
            using var channel = connection.CreateModel();
            channel.ExchangeDeclare(exchange: _configuration["RabbitMQ:PreferencesExchangeName"], type: ExchangeType.Fanout);
                
            var message = JsonSerializer.Serialize(onlinePreferenceFriendsDto);
            var body = Encoding.UTF8.GetBytes(message);

            channel.BasicPublish(exchange: _configuration["RabbitMQ:PreferencesExchangeName"],
                routingKey: "",
                basicProperties: null,
                body: body);
        }
        catch (Exception e) {
            _logger.LogError("Error while sending notification to RabbitMQ: {Message}", e.Message);
        }
        return Task.CompletedTask;
    }
}