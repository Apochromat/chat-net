using ChatNet.Common.Interfaces;
using ChatNet.Notification.BLL.Services;
using ChatNet.Notification.DAL;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace ChatNet.Notification.BLL.Extensions; 

/// <summary>
/// Service dependency extension
/// </summary>
public static class ServiceDependencyExtension {
    /// <summary>
    /// Add Notification BLL service dependencies
    /// </summary>
    /// <param name="services"></param>
    /// <param name="configuration"></param>
    /// <returns></returns>
    public static IServiceCollection AddNotificationServiceDependencies(this IServiceCollection services, IConfiguration configuration) {
        services.AddDbContext<NotificationDbContext>(options => 
            options.UseNpgsql(configuration.GetConnectionString("NotificationDatabase")));
        services.AddHostedService<RabbitMqNotificationsListenerService>();
        services.AddHostedService<RabbitMqPreferencesListenerService>();
        services.AddScoped<IConnectionManagerService, ConnectionManagerService>();
        services.AddScoped<IOnlinePreferencesManagerService, OnlinePreferenceManagerService>();
        return services;
    }
}