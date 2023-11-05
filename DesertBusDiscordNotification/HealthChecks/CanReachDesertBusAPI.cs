using System.Text.Json;
using Microsoft.Extensions.Diagnostics.HealthChecks;

namespace DesertBusDiscordNotification.HealthChecks;

public class CanReachDesertBusAPI : IHealthCheck
{
    private readonly HttpClient _client;

    public CanReachDesertBusAPI(IServiceProvider services)
    {
        var factory = services.GetRequiredService<IHttpClientFactory>();
        _client = factory.CreateClient("DesertBusAPI");
    }
    
    public async Task<HealthCheckResult> CheckHealthAsync(HealthCheckContext context, CancellationToken cancellationToken = new())
    {
        var response = await _client.GetAsync("prizes/giveaway", cancellationToken);
        var responseText = await response.Content.ReadAsStringAsync(cancellationToken);
        if (!response.IsSuccessStatusCode)
        {
            return await Task.FromResult(HealthCheckResult.Unhealthy("Desert Bus API is unreachable: " + response.StatusCode + Environment.NewLine + responseText));
        }

        JsonElement isValidJson = System.Text.Json.JsonSerializer.Deserialize<dynamic>(responseText);
        if (isValidJson.ValueKind != JsonValueKind.Object)
        {
            return await Task.FromResult(HealthCheckResult.Unhealthy($"Desert Bus API is reachable but didn't return JSON object: {Environment.NewLine}{(int)response.StatusCode}{Environment.NewLine}{responseText}"));
        }

        return await Task.FromResult(HealthCheckResult.Healthy("Desert Bus API is reachable"));

    }
}