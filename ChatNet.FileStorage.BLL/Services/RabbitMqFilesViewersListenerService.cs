using System.Text;
using System.Text.Json;
using ChatNet.Common.DataTransferObjects;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;

namespace ChatNet.FileStorage.BLL.Services; 

/// <summary>
/// Service that listens to the RabbitMQ queue for new file viewers.
/// </summary>
public class RabbitMqFilesViewersListenerService : BackgroundService {
    private readonly IConnection _connection;
    private readonly IModel _channel;
    private readonly IConfiguration _configuration;
    private readonly ILogger<RabbitMqFilesViewersListenerService> _logger;
    private readonly IServiceScopeFactory _serviceScopeFactory;

    /// <summary>
    /// Constructor
    /// </summary>
    /// <param name="configuration"></param>
    /// <param name="logger"></param>
    /// <param name="serviceScopeFactory"></param>
    public RabbitMqFilesViewersListenerService(IConfiguration configuration, 
        ILogger<RabbitMqFilesViewersListenerService> logger,
        IServiceScopeFactory serviceScopeFactory) {
        _configuration = configuration;
        _logger = logger;
        _serviceScopeFactory = serviceScopeFactory;

        var factory = new ConnectionFactory() { HostName = _configuration["RabbitMQ:HostName"] };
        _connection = factory.CreateConnection();
        _channel = _connection.CreateModel();
        _channel.ExchangeDeclare(exchange: _configuration["RabbitMQ:FileViewersExchangeName"], type: ExchangeType.Fanout);

        var queueName = _channel.QueueDeclare(queue: _configuration["RabbitMQ:FileViewersQueueName"]).QueueName;
        _channel.QueueBind(queue: queueName,
            exchange: _configuration["RabbitMQ:FileViewersExchangeName"],
            routingKey: string.Empty);
    }

    /// <summary>
    /// Executes service
    /// </summary>
    /// <param name="stoppingToken"></param>
    /// <returns></returns>
    /// <exception cref="InvalidOperationException"></exception>
    protected override Task ExecuteAsync(CancellationToken stoppingToken) {
        stoppingToken.ThrowIfCancellationRequested();

        var consumer = new EventingBasicConsumer(_channel);
        consumer.Received += async (_, eventArgs) => {
            try {
                var rawMessage = Encoding.UTF8.GetString(eventArgs.Body.ToArray());
                var message = JsonSerializer.Deserialize<FilesViewersDto>(rawMessage);
                if (message == null) throw new InvalidOperationException();

                using (var scope = _serviceScopeFactory.CreateScope()) {
                    var service = scope.ServiceProvider.GetRequiredService<FileStorageService>();

                    await service.SetViewerAsync(message, null, false);
                }

                _logger.LogInformation($"Received message");
            }
            catch (Exception ex) {
                _logger.LogError(ex, "Error processing message");
            }
        };

        _channel.BasicConsume(queue: _configuration["RabbitMQ:FileViewersQueueName"], autoAck: true, consumer: consumer);

        return Task.CompletedTask;
    }

    /// <summary>
    /// Disposes service
    /// </summary>
    public override void Dispose() {
        _channel.Close();
        _connection.Close();
        base.Dispose();
    }
}