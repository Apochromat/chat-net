using System.Text;
using System.Text.Json;
using ChatNet.Backend.DAL;
using ChatNet.Backend.DAL.Entities;
using ChatNet.Common.DataTransferObjects;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;

namespace ChatNet.Backend.BLL.Services; 

public class BackendReceiverService : BackgroundService {
     private readonly IConnection _connection;
    private readonly IModel _channel;
    private readonly IConfiguration _configuration;
    private readonly ILogger<BackendReceiverService> _logger;
    private readonly IServiceScopeFactory _serviceScopeFactory;

    /// <summary>
    /// Constructor
    /// </summary>
    /// <param name="configuration"></param>
    /// <param name="logger"></param>
    /// <param name="serviceScopeFactory"></param>
    public BackendReceiverService(IConfiguration configuration, ILogger<BackendReceiverService> logger, IServiceScopeFactory serviceScopeFactory) {
        _configuration = configuration;
        _logger = logger;
        _serviceScopeFactory = serviceScopeFactory;

        var factory = new ConnectionFactory() { HostName = _configuration["RabbitMQ:HostName"] };
        _connection = factory.CreateConnection();
        _channel = _connection.CreateModel();
        _channel.ExchangeDeclare(exchange: _configuration["RabbitMQ:UsersExchangeName"], type: ExchangeType.Fanout);

        var queueName = _channel.QueueDeclare(queue: _configuration["RabbitMQ:UsersQueueName"]).QueueName;
        _channel.QueueBind(queue: queueName,
            exchange: _configuration["RabbitMQ:UsersExchangeName"],
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
                var message = JsonSerializer.Deserialize<NotificationMessageDto>(rawMessage);
                if (message == null) throw new InvalidOperationException();

                using (var scope = _serviceScopeFactory.CreateScope()) {
                    var dbContext = scope.ServiceProvider.GetRequiredService<BackendDbContext>();
                    await dbContext.Users.AddAsync(new UserBackend {
                        Id = message.NewUserId
                    },stoppingToken);
                
                    await dbContext.SaveChangesAsync(stoppingToken);
                }
                _logger.LogInformation($"Received message");
            }
            catch (Exception ex) {
                _logger.LogError(ex, "Error processing message");
            }
        };

        _channel.BasicConsume(queue: _configuration["RabbitMQ:UsersQueueName"], autoAck: true, consumer: consumer);

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