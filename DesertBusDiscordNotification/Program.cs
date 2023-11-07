using System.Globalization;
using System.Reflection;
using DesertBusDiscordNotification.Client;
using DesertBusDiscordNotification.HealthChecks;
using DesertBusDiscordNotification.Services;
using DesertBusDiscordNotification.Services.Sender;
using Microsoft.Extensions.Diagnostics.HealthChecks;

CultureInfo.CurrentCulture = new CultureInfo("en-US");
CultureInfo.CurrentUICulture = new CultureInfo("en-US");

var builder = WebApplication.CreateBuilder(args);
builder.Configuration
    .AddJsonFile("appsettings.json", optional: false, reloadOnChange: true)
    .AddJsonFile($"appsettings.{builder.Environment.EnvironmentName}.json", optional: true)
    .AddEnvironmentVariables()
    .Build();
    
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
    .Configure<DesertBusDiscordNotification.Options.Discord>(builder.Configuration.GetSection(nameof(DesertBusDiscordNotification.Options.Discord)));

builder.Services
    .AddSingleton<ISender, DiscordSender>()
    .AddSingleton<GiveawayTracker>();

var app = builder.Build();
app.MapGet("/", () => "OK");
app.MapGet("/version" , () => Assembly.GetExecutingAssembly().GetCustomAttribute<AssemblyInformationalVersionAttribute>()?.InformationalVersion); 
app.MapGet("/send" , () =>
{
    app.Services
        .GetRequiredService<ISender>()
        .SendAsync("test", 1.23m, "https://desertbus.org/_nuxt/1630da356cd42da9eff8d517e002743b.png").Wait();
}); 
app.MapHealthChecks("/health");
app.Run();

namespace DesertBusDiscordNotification
{
    public abstract class Program;
}