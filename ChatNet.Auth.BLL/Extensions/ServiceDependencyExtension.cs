using ChatNet.Auth.BLL.Services;
using ChatNet.Auth.DAL;
using ChatNet.Common.Interfaces;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace ChatNet.Auth.BLL.Extensions; 

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
    public static IServiceCollection AddAuthBlServiceDependencies(this IServiceCollection services, IConfiguration configuration) {
        services.AddDbContext<AuthDbContext>(options => 
            options.UseNpgsql(configuration.GetConnectionString("AuthDatabase")));
        services.AddScoped<IAuthService, AuthService>();
        services.AddScoped<IAccountService, AccountService>();
        services.AddScoped<IMessageSenderService, MessageSernderService>();
        return services;
    }
}