using DesertBusDiscordNotification.Client;
using DesertBusDiscordNotification.Client.Models;
using DesertBusDiscordNotification.Services.Sender;

namespace DesertBusDiscordNotification.Services;

public class GiveawayTracker(ISender sender, IDesertBusAPIClient apiClient, ILogger<GiveawayTracker> logger)
{
    private ISender Sender { get; } = sender;
    private IDesertBusAPIClient ApiClient { get; } = apiClient;
    private ILogger<GiveawayTracker> Logger { get; } = logger;

    private readonly Dictionary<int, Prize> _state = new();
    
    private void OnPrizeNotInactive(int id)
    {
        Logger.LogInformation("Prize {Id} is not inactive, sending notification...", id);
        Sender.SendAsync(
            _state[id].title,
            0,
            _state[id].image
        );
    }

    public async Task UpdateAsync(CancellationToken cancellationToken = default)
    {
        Logger.LogInformation("Updating giveaway state...");
        var updatedGiveaways = await ApiClient.GetGiveaways(cancellationToken);
        if (updatedGiveaways is null)
        {
            return;
        }
        
        foreach (var updatedGiveaway in updatedGiveaways)
        {
            if ((!_state.TryGetValue(updatedGiveaway.id, out var value) || value.state == updatedGiveaway.state) && updatedGiveaway.state == "inactive")
            {
                Logger.LogTrace("Prize {Id} is inactive, skipping...", updatedGiveaway.id);
                continue;
            }

            _state[updatedGiveaway.id] = updatedGiveaway;
            OnPrizeNotInactive(updatedGiveaway.id);
        }
    }
}