using DesertBusDiscordNotification.Client;
using DesertBusDiscordNotification.Client.Models;
using DesertBusDiscordNotification.Services.Sender;

namespace DesertBusDiscordNotification.Services;

public class GiveawayTracker(ISender sender, IDesertBusAPIClient apiClient)
{
    private ISender Sender { get; } = sender;
    private IDesertBusAPIClient ApiClient { get; } = apiClient;

    private readonly Dictionary<int, Prize> _state = new();
    
    private void OnPrizeNotInactive(int id)
    {
        Sender.SendAsync(
            _state[id].title,
            0,
            _state[id].image
        );
    }

    public async Task Update(CancellationToken cancellationToken)
    {
        var updatedGiveaways = await ApiClient.GetGiveaways(cancellationToken);
        if (updatedGiveaways is null)
        {
            return;
        }
        
        foreach (var updatedGiveaway in updatedGiveaways)
        {
            if ((!_state.TryGetValue(updatedGiveaway.id, out var value) || value.state == updatedGiveaway.state) && updatedGiveaway.state == "inactive")
            {
                continue;
            }

            _state[updatedGiveaway.id] = updatedGiveaway;
            OnPrizeNotInactive(updatedGiveaway.id);
        }
    }
}