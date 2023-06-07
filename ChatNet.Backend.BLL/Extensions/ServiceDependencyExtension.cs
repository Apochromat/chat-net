using ChatNet.Backend.BLL.Services;
using ChatNet.Backend.DAL;
using ChatNet.Common.Interfaces;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
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
    public static IServiceCollection AddBackendBlServiceDependencies(this IServiceCollection services, IConfiguration configuration) {
        services.AddDbContext<BackendDbContext>(options => 
            options.UseNpgsql(configuration.GetConnectionString("BackendDatabase")));
        services.AddScoped<IChatService, ChatService>();
        services.AddScoped<IMessageService, MessageService>();
        services.AddScoped<IPermissionCheckService, PermissionCheckService>();
        services.AddScoped<IFriendService, FriendService>();
        services.AddHostedService<BackendReceiverService>();
        services.AddScoped<INotificationQueueService, NotificationQueueService>();
        services.AddScoped<IFilesQueueService, FilesQueueService>();
        return services;
    }
}