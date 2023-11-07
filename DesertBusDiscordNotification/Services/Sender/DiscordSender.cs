using System.Text;
using System.Text.Json;
using DesertBusDiscordNotification.Options;
using Microsoft.Extensions.Options;

namespace DesertBusDiscordNotification.Services.Sender;

// construct a discord client and send message to channel
internal class DiscordSender(HttpClient httpClient, IOptions<Discord> options) : ISender
{
    private readonly Discord _options = options.Value;

    public async Task SendAsync(string itemName, decimal amount, string imageUrl)
    {
        await httpClient.PostAsync(_options.WebhookURL, new StringContent(JsonSerializer.Serialize(new
        {
            embeds = new[]
            {
                new
                {
                    title = $"Giveaway started: '{itemName}'",
                    description = "```"+string.Join("\n", Enumerable.Range(1, 10).Select(i => $"{i,2} entr{(i==1?"y  ":"ies")}: ${amount * i,2}"))+"```",
                    image = new
                    {
                        url = imageUrl
                    }
                }
            }
        }), Encoding.UTF8, "application/json"));
    }
}