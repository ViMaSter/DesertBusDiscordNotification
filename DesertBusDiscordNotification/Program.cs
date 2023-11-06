using System.Reflection;
using DesertBusDiscordNotification.Client;
using DesertBusDiscordNotification.HealthChecks;
using DesertBusDiscordNotification.Services;
using DesertBusDiscordNotification.Services.Sender;
using Microsoft.Extensions.Diagnostics.HealthChecks;

var builder = WebApplication.CreateBuilder(args);
builder.Services
    .AddHttpClient<IDesertBusAPIClient, DesertBusAPIClient>();

builder.Services
    .AddHealthChecks()
    .Add(new HealthCheckRegistration(
        nameof(CanReachDesertBusAPI),
        serviceProvider => new CanReachDesertBusAPI(serviceProvider),
        HealthStatus.Unhealthy,
        new[] { "ready" }
    ));

builder.Services
    .AddSingleton<ISender, DiscordSender>()
    .AddSingleton<GiveawayTracker>();

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