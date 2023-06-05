using ChatNet.Notification.BLL.Jobs;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Quartz;

namespace ChatNet.Notification.BLL.Extensions; 

/// <summary>
/// Extension for Quartz configuration.
/// </summary>
public static class QuartzConfigureExtension {
    /// <summary>
    /// Quartz configuration.
    /// </summary>
    /// <param name="services"></param>
    /// <param name="configuration"></param>
    /// <returns></returns>
    public static IServiceCollection ConfigureQuartz(this IServiceCollection services, IConfiguration configuration) {
        services.AddQuartz(q => {
            q.UseMicrosoftDependencyInjectionJobFactory();
            
            var notificationMessagesSendingJobKey = new JobKey("NotificationMessagesSendingJob");
            q.AddJob<NotificationMessagesSendingJob>(opts => opts.WithIdentity(notificationMessagesSendingJobKey));

            q.AddTrigger(opts => opts
                .ForJob(notificationMessagesSendingJobKey)
                .WithIdentity("NotificationMessagesSendingJob-trigger")
                .WithCronSchedule(configuration.GetSection("Jobs")
                    .GetSection("NotificationMessagesSendingJob")
                    .GetValue<string>("CronExpression") ?? "* */5 * ? * *")
            );
            
            var connectionClearJobKey = new JobKey("ConnectionClearJob");
            q.AddJob<ConnectionClearJob>(opts => opts.WithIdentity(connectionClearJobKey));

            q.AddTrigger(opts => opts
                .ForJob(connectionClearJobKey)
                .WithIdentity("ConnectionClearJob-trigger")
                .WithCronSchedule(configuration.GetSection("Jobs")
                    .GetSection("ConnectionClearJob")
                    .GetValue<string>("CronExpression") ?? "* */5 * ? * *")
            );
            
        });
        services.AddQuartzHostedService(q => q.WaitForJobsToComplete = true);
        return services;
    }
}