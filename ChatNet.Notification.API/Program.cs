using ChatNet.Common.Extensions;
using ChatNet.Common.Interfaces;
using ChatNet.Notification.API.Hubs;
using ChatNet.Notification.API.Services;
using ChatNet.Notification.BLL.Extensions;
using Microsoft.AspNetCore.SignalR;
using Serilog;

var builder = WebApplication.CreateBuilder(args);

// Add Cors Policy
builder.Services.AddCors(options => {
    options.AddDefaultPolicy(
        policy => {
            policy.WithOrigins("null")
                .AllowAnyHeader()
                .AllowAnyMethod()
                .AllowCredentials()
                .SetIsOriginAllowed(_ => true);
        });
});

// Add services to the container.
builder.Services.AddControllers();
builder.Services.AddNotificationServiceDependencies(builder.Configuration);

// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

builder.Services.AddAuthorization();
builder.Services.AddJwtAuthentication(builder.Configuration);

// SignalR
builder.Services.AddScoped<INotificationService, NotificationService>();
builder.Services.AddSingleton<IUserIdProvider, SignalRUserIdProvider>();
builder.Services.AddSignalR();

// Quartz
builder.Services.ConfigureQuartz(builder.Configuration);

// Serilog
var logger = new LoggerConfiguration()
    .ReadFrom.Configuration(builder.Configuration)
    .Enrich.FromLogContext()
    .CreateLogger();
builder.Logging.ClearProviders();
builder.Logging.AddSerilog(logger);

var app = builder.Build();

await app.MigrateDbAsync();
await app.CloseOpenedConnectionsAsync();

app.UseCors();

app.UseSwagger();
app.UseSwaggerUI();

//app.UseHttpsRedirection();

app.UseAuthorization();
app.UseAuthentication();

app.MapControllers();
app.MapHub<NotificationHub>("api/notification/hub");

app.Run();