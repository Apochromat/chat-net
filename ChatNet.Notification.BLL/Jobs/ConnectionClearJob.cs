using ChatNet.Notification.DAL;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Quartz;

namespace ChatNet.Notification.BLL.Jobs; 

/// <summary>
/// Job for clearing connections
/// </summary>
public class ConnectionClearJob : IJob {
    private readonly ILogger<ConnectionClearJob> _logger;
    private readonly NotificationDbContext _dbContext;
    private readonly IConfiguration _configuration;

    /// <summary>
    /// Constructor
    /// </summary>
    /// <param name="logger"></param>
    /// <param name="dbContext"></param>
    /// <param name="configuration"></param>
    public ConnectionClearJob(ILogger<ConnectionClearJob> logger, NotificationDbContext dbContext, 
        IConfiguration configuration) {
        _dbContext = dbContext;
        _configuration = configuration;
        _logger = logger;
    }

    /// <summary>
    /// Executes job
    /// </summary>
    /// <param name="context"></param>
    public async Task Execute(IJobExecutionContext context) {
        _logger.LogInformation("NotificationSendingJob is running");
        var notifications = _dbContext.Connections
            .Where(m => m.DisconnectedAt.HasValue && m.DisconnectedAt.Value.AddDays(
                            _configuration.GetSection("Jobs")
                                .GetSection("ConnectionClearJob")
                                .GetValue<int>("ConnectionRottenTimeInDays")) < DateTime.UtcNow) 
            .ToList();

        _dbContext.Connections.RemoveRange(notifications);
        await _dbContext.SaveChangesAsync();
    }
}