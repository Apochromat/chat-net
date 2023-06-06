using ChatNet.Common.Interfaces;
using ChatNet.FileStorage.BLL.Services;
using ChatNet.FileStorage.DAL;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace ChatNet.FileStorage.BLL.Extensions; 

/// <summary>
/// Service dependency extension
/// </summary>
public static class ServiceDependencyExtension {
    /// <summary>
    /// Add Backend BLL service dependencies
    /// </summary>
    /// <param name="services"></param>
    /// <param name="configuration"></param>
    /// <returns></returns>
    public static IServiceCollection AddFileStorageServiceDependencies(this IServiceCollection services, IConfiguration configuration) {
        services.AddDbContext<FileStorageDbContext>(options => 
            options.UseNpgsql(configuration.GetConnectionString("FileStorageDatabase")));
        services.AddHostedService<RabbitMqFilesViewersListenerService>();
        services.AddScoped<IFileStorageService, FileStorageService>();
        return services;
    }
}