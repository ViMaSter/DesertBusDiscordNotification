using System.Text.Json;
using DesertBusDiscordNotification.Client;
using Microsoft.Extensions.Diagnostics.HealthChecks;

namespace DesertBusDiscordNotification.HealthChecks;

public class CanReachDesertBusAPI : IHealthCheck
{
    private readonly DesertBusAPI _client;

    public CanReachDesertBusAPI(IServiceProvider services)
    {
        _client = services.GetRequiredService<DesertBusAPI>();
    }
    
    public async Task<HealthCheckResult> CheckHealthAsync(HealthCheckContext context, CancellationToken cancellationToken = new())
    {
        var responseText = await _client.GetGiveways(cancellationToken);

        JsonElement isValidJson = JsonSerializer.Deserialize<dynamic>(responseText);
        if (isValidJson.ValueKind != JsonValueKind.Object)
        {
            return await Task.FromResult(HealthCheckResult.Unhealthy($"Desert Bus API is reachable but didn't return JSON object: {Environment.NewLine}{responseText}"));
        }

        return await Task.FromResult(HealthCheckResult.Healthy("Desert Bus API is reachable"));

    }
}