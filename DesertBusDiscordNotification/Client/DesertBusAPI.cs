using System.Reflection;

namespace DesertBusDiscordNotification.Client;
public class DesertBusAPI
{
    private readonly HttpClient _httpClient;

    public DesertBusAPI(HttpClient httpClient)
    {
        _httpClient = httpClient;
        _httpClient.BaseAddress = new Uri("https://desertbus.org/wapi/");
        _httpClient.DefaultRequestHeaders.Add("User-Agent", $"DesertBusDiscordNotification/{Assembly.GetExecutingAssembly().GetCustomAttribute<AssemblyInformationalVersionAttribute>()?.InformationalVersion}");
    }

    public async Task<string> GetGiveways(CancellationToken cancellationToken = new())
    {
        var response = await _httpClient.GetAsync("prizes/giveaway");
        response.EnsureSuccessStatusCode();
        return await response.Content.ReadAsStringAsync(cancellationToken);;
    }
}
