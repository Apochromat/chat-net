using ChatNet.Call.BLL.Services;
using ChatNet.Call.DAL;
using ChatNet.Common.Interfaces;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace ChatNet.Call.BLL.Extensions; 

/// <summary>
/// Service dependency extension
/// </summary>
public static class ServiceDependencyExtension {
    /// <summary>
    /// Add Call BLL service dependencies
    /// </summary>
    /// <param name="services"></param>
    /// <param name="configuration"></param>
    /// <returns></returns>
    public static IServiceCollection AddCallServiceDependencies(this IServiceCollection services, IConfiguration configuration) {
        services.AddDbContext<CallDbContext>(options => 
            options.UseNpgsql(configuration.GetConnectionString("CallDatabase")));
        services.AddScoped<ICallService, CallService>();
        return services;
    }
}