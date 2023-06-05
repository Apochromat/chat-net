using ChatNet.Notification.BLL.Jobs;
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
    /// <returns></returns>
    public static IServiceCollection ConfigureQuartz(this IServiceCollection services) {
        services.AddQuartz(q => {
            q.UseMicrosoftDependencyInjectionJobFactory();
            
            var jobKey = new JobKey("NotificationSendingJob");
            q.AddJob<NotificationMessagesSendingJob>(opts => opts.WithIdentity(jobKey));

            q.AddTrigger(opts => opts
                .ForJob(jobKey)
                .WithIdentity("NotificationSendingJob-trigger")
                //This Cron interval can be described as "run every minute" (when second is zero)
                .WithCronSchedule("0 * * ? * *")
            );
        });
        services.AddQuartzHostedService(q => q.WaitForJobsToComplete = true);
        return services;
    }
}