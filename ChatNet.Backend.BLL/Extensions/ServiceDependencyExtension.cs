using ChatNet.Backend.BLL.Services;
using ChatNet.Common.Interfaces;
using Microsoft.Extensions.DependencyInjection;

namespace ChatNet.Backend.BLL.Extensions; 

/// <summary>
/// Service dependency extension
/// </summary>
public static class ServiceDependencyExtension {
    /// <summary>
    /// Add Backend BLL service dependencies
    /// </summary>
    /// <param name="services"></param>
    /// <returns></returns>
    public static IServiceCollection AddBackendServiceDependencies(this IServiceCollection services) {
        services.AddScoped<INotificationQueueService, NotificationQueueService>();
        services.AddScoped<IFilesQueueService, FilesQueueService>();
        return services;
    }
}