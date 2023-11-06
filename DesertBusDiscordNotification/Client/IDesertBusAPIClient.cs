namespace DesertBusDiscordNotification.Client;

public interface IDesertBusAPIClient
{
    Task<Models.Prize[]?> GetGiveaways(CancellationToken cancellationToken);
}