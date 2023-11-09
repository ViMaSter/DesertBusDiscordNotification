using DesertBusDiscordNotification.Client;
using DesertBusDiscordNotification.Client.Models;
using DesertBusDiscordNotification.Services.Sender;
using Microsoft.Extensions.Logging.Abstractions;
using NSubstitute;

namespace DesertBusDiscordNotification.Test.Services.GiveawayTracker;

public class CallsSender
{
    private static readonly Prize PrizeInactive = new(1, "", "inactive", "image", "title", -1, "", "", "", "", null!);
    private static readonly Prize PrizeActive = PrizeInactive with { state = "active" };
    
    [Test]
    public async Task GiveawayTracker_SkipsSender_IfNewItemIsInactive()
    {
        var sender = Substitute.For<ISender>();
        var apiClient = Substitute.For<IDesertBusAPIClient>();
        var tracker = new DesertBusDiscordNotification.Services.GiveawayTracker(sender, apiClient, NullLogger<DesertBusDiscordNotification.Services.GiveawayTracker>.Instance);

        apiClient.GetGiveaways(Arg.Any<CancellationToken>()).Returns(new[] { PrizeInactive });
        await tracker.UpdateAsync(CancellationToken.None);
        await sender.DidNotReceive().SendAsync(PrizeInactive.title, 0, PrizeInactive.image);
    }
    
    [Test]
    public async Task GiveawayTracker_UsesSender_IfNewItemIsActive()
    {
        var sender = Substitute.For<ISender>();
        var apiClient = Substitute.For<IDesertBusAPIClient>();
        var tracker = new DesertBusDiscordNotification.Services.GiveawayTracker(sender, apiClient, NullLogger<DesertBusDiscordNotification.Services.GiveawayTracker>.Instance);
        
        apiClient.GetGiveaways(Arg.Any<CancellationToken>()).Returns(new[] { PrizeActive });
        await tracker.UpdateAsync(CancellationToken.None);
        await sender.Received().SendAsync(PrizeInactive.title, 0, PrizeInactive.image);        
    }
    
    [Test]
    public async Task GiveawayTracker_UsesSender_IfExistingItemBecomesActive()
    {
        var sender = Substitute.For<ISender>();
        var apiClient = Substitute.For<IDesertBusAPIClient>();
        var tracker = new DesertBusDiscordNotification.Services.GiveawayTracker(sender, apiClient, NullLogger<DesertBusDiscordNotification.Services.GiveawayTracker>.Instance);

        apiClient.GetGiveaways(Arg.Any<CancellationToken>()).Returns(new[] { PrizeInactive });
        await tracker.UpdateAsync(CancellationToken.None);
        await sender.DidNotReceive().SendAsync(PrizeInactive.title, 0, PrizeInactive.image);
        
        apiClient.GetGiveaways(Arg.Any<CancellationToken>()).Returns(new[] { PrizeActive });
        await tracker.UpdateAsync(CancellationToken.None);
        await sender.Received().SendAsync(PrizeInactive.title, 0, PrizeInactive.image);  
    }
}