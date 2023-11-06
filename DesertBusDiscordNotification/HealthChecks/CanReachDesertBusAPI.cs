using DesertBusDiscordNotification.Client;
using Microsoft.Extensions.Diagnostics.HealthChecks;

namespace DesertBusDiscordNotification.HealthChecks;

public class CanReachDesertBusAPI(IServiceProvider services) : IHealthCheck
{
    private readonly IDesertBusAPIClient _client = services.GetRequiredService<IDesertBusAPIClient>();

    public async Task<HealthCheckResult> CheckHealthAsync(HealthCheckContext context, CancellationToken cancellationToken = new())
    {
        var response = await _client.GetGiveaways(cancellationToken);

        if (response == null)
        {
            return await Task.FromResult(HealthCheckResult.Unhealthy("Desert Bus API is reachable but response couldn't be parsed"));
        }

        return await Task.FromResult(HealthCheckResult.Healthy("Desert Bus API is reachable"));

    }
}