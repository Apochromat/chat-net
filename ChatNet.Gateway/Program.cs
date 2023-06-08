using Ocelot.DependencyInjection;
using Ocelot.Middleware;

var builder = WebApplication.CreateBuilder(args);

// Add CORS
builder.Services.AddCors(options => {
    options.AddDefaultPolicy(policyBuilder => policyBuilder
        .WithOrigins("localhost:3000")
        .AllowAnyMethod()
        .AllowAnyHeader()
        .AllowCredentials()
        .SetIsOriginAllowed((host) => true));
});

builder.Configuration.AddJsonFile(builder.Environment.IsDevelopment()
    ? "ocelot.Development.json"
    : "ocelot.Production.json");

builder.Services.AddControllers();
builder.Services.AddOcelot();

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
builder.Services.AddSwaggerForOcelot(builder.Configuration);

var app = builder.Build();

app.UseCors();

app.UseSwagger();
app.UseSwaggerForOcelotUI(opt => {
    opt.PathToSwaggerGenerator = "/swagger/docs";
});

//app.UseHttpsRedirection();

app.UseAuthorization();

app.UseRouting();
app.MapControllers();

app.UseWebSockets();
app.UseOcelot().Wait();

app.Run();