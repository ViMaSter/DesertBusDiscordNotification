using System.Reflection;
using DesertBusDiscordNotification.HealthChecks;
using Microsoft.Extensions.Diagnostics.HealthChecks;

var builder = WebApplication.CreateBuilder(args);
builder.Services
    .AddHttpClient("DesertBusAPI", client =>
    {
        client.BaseAddress = new Uri("https://desertbus.org/wapi/");
        client.DefaultRequestHeaders.Add("User-Agent", $"DesertBusDiscordNotification/{Assembly.GetExecutingAssembly().GetCustomAttribute<AssemblyInformationalVersionAttribute>()?.InformationalVersion}");
    });

builder.Services
    .AddHealthChecks()
    .Add(new HealthCheckRegistration(
        nameof(CanReachDesertBusAPI),
        serviceProvider => new CanReachDesertBusAPI(serviceProvider),
        HealthStatus.Unhealthy,
        new[] { "ready" }
    ));

var app = builder.Build();
app.MapGet("/", () => "OK");
app.MapGet("/version" , () => Assembly.GetExecutingAssembly().GetCustomAttribute<AssemblyInformationalVersionAttribute>()?.InformationalVersion); 
app.MapHealthChecks("/health");
app.Run();

namespace DesertBusDiscordNotification
{
    public partial class Program
    { }
}