using System.Reflection;
using System.Text.Json;

namespace DesertBusDiscordNotification.Client;

public class DesertBusAPIClient : IDesertBusAPIClient
{
    private readonly HttpClient _httpClient;

    public DesertBusAPIClient(HttpClient httpClient)
    {
        _httpClient = httpClient;
        _httpClient.BaseAddress = new Uri("https://desertbus.org/wapi/");
        _httpClient.DefaultRequestHeaders.Add("User-Agent", $"DesertBusDiscordNotification/{Assembly.GetExecutingAssembly().GetCustomAttribute<AssemblyInformationalVersionAttribute>()?.InformationalVersion}");
    }

    public async Task<Models.Prize[]?> GetGiveaways(CancellationToken cancellationToken)
    {
        var response = await _httpClient.GetAsync("prizes/giveaway", cancellationToken);
        response.EnsureSuccessStatusCode();
        var text = await response.Content.ReadAsStringAsync(cancellationToken);
        return JsonSerializer.Deserialize<Models.GivewayResponse>(text)?.prizes;
    }
}
