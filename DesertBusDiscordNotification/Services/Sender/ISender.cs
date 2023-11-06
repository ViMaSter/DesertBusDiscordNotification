namespace DesertBusDiscordNotification.Services.Sender;

public interface ISender
{
    public Task SendAsync(string itemName, decimal amount, string imageUrl);
}