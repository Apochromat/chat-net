using Ocelot.DependencyInjection;
using Ocelot.Middleware;

var builder = WebApplication.CreateBuilder(args);

if (builder.Environment.IsDevelopment()) {
    builder.Configuration.AddJsonFile("ocelot.Production.json");
}
else {
    builder.Configuration.AddJsonFile("ocelot.Development.json");
}

builder.Services.AddControllers();
builder.Services.AddOcelot();

var app = builder.Build();

//app.UseHttpsRedirection();

app.UseAuthorization();

app.UseRouting();
app.MapControllers();

app.UseOcelot().Wait();

app.Run();