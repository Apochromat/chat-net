using ChatNet.Common.DataTransferObjects;
using ChatNet.Common.Interfaces;
using ChatNet.Notification.DAL;
using Microsoft.Extensions.Logging;
using Quartz;

namespace ChatNet.Notification.BLL.Jobs; 

/// <summary>
/// Job for sending notification messages
/// </summary>
public class NotificationMessagesSendingJob : IJob {
    private readonly ILogger<NotificationMessagesSendingJob> _logger;
    private readonly NotificationDbContext _dbContext;
    private readonly INotificationService _notificationService;

    /// <summary>
    /// Constructor
    /// </summary>
    /// <param name="logger"></param>
    /// <param name="dbContext"></param>
    /// <param name="notificationService"></param>
    public NotificationMessagesSendingJob(ILogger<NotificationMessagesSendingJob> logger, NotificationDbContext dbContext,
        INotificationService notificationService) {
        _dbContext = dbContext;
        _notificationService = notificationService;
        _logger = logger;
    }

    /// <summary>
    /// Executes job
    /// </summary>
    /// <param name="context"></param>
    public async Task Execute(IJobExecutionContext context) {
        _logger.LogInformation("NotificationSendingJob is running");
        var notifications = _dbContext.Messages.Where(m => !m.DeliveredAt.HasValue).ToList();
        foreach (var notification in notifications) {
            var sent = await _notificationService.SendNotificationAsync(new NotificationMessageDto() {
                ReceiverId = notification.ReceiverId,
                SenderId = notification.SenderId,
                Type = notification.Type,
                Title = notification.Title,
                Text = notification.Text,
                CreatedAt = notification.CreatedAt,
                ChatId = notification.ChatId,
                CallId = notification.CallId
            });

            if (sent) {
                notification.DeliveredAt = DateTime.UtcNow;
            }
        }

        await _dbContext.SaveChangesAsync();
    }
}