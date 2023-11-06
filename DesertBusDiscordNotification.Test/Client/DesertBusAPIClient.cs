namespace DesertBusDiscordNotification.Test.Client;

public class DesertBusAPIClient
{
    [Test]
    public void GiveawaysArentEmpty()
    {
        var client = new DesertBusDiscordNotification.Client.DesertBusAPIClient(new HttpClient());
        var giveaways = client.GetGiveaways(CancellationToken.None).Result;
        Assert.That(giveaways, Is.Not.Empty);
    }
}