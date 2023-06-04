using Ocelot.DependencyInjection;
using Ocelot.Middleware;

var builder = WebApplication.CreateBuilder(args);

builder.Configuration.AddJsonFile(builder.Environment.IsDevelopment()
    ? "ocelot.Production.json"
    : "ocelot.Development.json");

builder.Services.AddControllers();
builder.Services.AddOcelot();

var app = builder.Build();

//app.UseHttpsRedirection();

app.UseAuthorization();

app.UseRouting();
app.MapControllers();

app.UseOcelot().Wait();

app.Run();