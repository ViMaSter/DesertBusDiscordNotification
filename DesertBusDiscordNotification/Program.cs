using System.Globalization;
using System.Reflection;
using System.Web;
using DesertBusDiscordNotification.Client;
using DesertBusDiscordNotification.HealthChecks;
using DesertBusDiscordNotification.Services;
using DesertBusDiscordNotification.Services.Sender;
using Hangfire;
using Hangfire.Dashboard.BasicAuthorization;
using Microsoft.Extensions.Diagnostics.HealthChecks;
using Microsoft.Extensions.Options;
using Newtonsoft.Json;

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
    .Configure<DesertBusDiscordNotification.Options.Discord>(builder.Configuration.GetSection(nameof(DesertBusDiscordNotification.Options.Discord)))
    .Configure<DesertBusDiscordNotification.Options.Hangfire>(builder.Configuration.GetSection(nameof(DesertBusDiscordNotification.Options.Hangfire)));

builder.Services
    .AddSingleton<ISender, DiscordSender>()
    .AddSingleton<GiveawayTracker>();

GlobalConfiguration.Configuration.UseInMemoryStorage();
builder.Services.AddHangfireServer();
builder.Services.AddHangfire((_, configuration) =>
{
    configuration.UseSimpleAssemblyNameTypeSerializer();
    configuration.UseRecommendedSerializerSettings();
});

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
app.MapPost("/track", async (HttpRequest request) =>
{
    var stringContent = "";
    using (var stream = new StreamReader(request.Body))
    {
        stringContent = await stream.ReadToEndAsync();
    }

    var body = HttpUtility.UrlDecode(stringContent);
    var tracker = request.HttpContext.RequestServices.GetRequiredService<GiveawayTracker>();
    var data = JsonConvert.DeserializeObject<dynamic>(body);
    int id = data.id;
    string action = data.action;
    return action switch
    {
        "add" when !tracker.AddTracking(id) => Results.BadRequest("Already tracking"),
        "add" => Results.NoContent(),
        "remove" when !tracker.RemoveTracking(id) => Results.BadRequest("Not tracking"), 
        "remove" => Results.NoContent(),
        _ => throw new Exception("Invalid action")
    };
});

var options = new DashboardOptions
{
    Authorization = new[]
    {
        new BasicAuthAuthorizationFilter(new BasicAuthAuthorizationFilterOptions
        {
            SslRedirect = false,
            RequireSsl = false,
            LoginCaseSensitive = true,
            Users = new []
            {
                new BasicAuthAuthorizationUser
                {
                    Login = "admin",
                    PasswordClear = app.Services.GetRequiredService<IOptions<DesertBusDiscordNotification.Options.Hangfire>>().Value.Password
                }
            }
        })
    },
};

app.UseHangfireDashboard("/hangfire", options);
RecurringJob.AddOrUpdate<GiveawayTracker>(nameof(GiveawayTracker), x => x.UpdateAsync(CancellationToken.None), Cron.Minutely());
RecurringJob.TriggerJob(nameof(GiveawayTracker));

app.Run();

namespace DesertBusDiscordNotification
{
    public abstract class Program;
}