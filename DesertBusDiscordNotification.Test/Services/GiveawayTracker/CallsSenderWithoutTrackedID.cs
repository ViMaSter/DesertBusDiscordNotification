using DesertBusDiscordNotification.Client;
using DesertBusDiscordNotification.Client.Models;
using DesertBusDiscordNotification.Services.Sender;
using Microsoft.Extensions.Logging.Abstractions;
using NSubstitute;

namespace DesertBusDiscordNotification.Test.Services.GiveawayTracker;

public class CallsSender
{
    private static readonly Prize PrizeActive = new(1, "", "active", "image", "title", 1.23m, "", "", "", "", null!);
    private static readonly Prize PrizeInactive = PrizeActive with { state = "inactive" };
    private static readonly Prize PrizeNoBid = PrizeActive with { bid = 0 };

    private Tuple<ISender, IDesertBusAPIClient, DesertBusDiscordNotification.Services.GiveawayTracker> GenerateTracker(int id)
    {
        var sender = Substitute.For<ISender>();
        var apiClient = Substitute.For<IDesertBusAPIClient>();
        var tracker = new DesertBusDiscordNotification.Services.GiveawayTracker(sender, apiClient, NullLogger<DesertBusDiscordNotification.Services.GiveawayTracker>.Instance);
        tracker.AddTracking(id);
        return Tuple.Create(sender, apiClient, tracker);
    }
    
    [Test]
    public async Task GiveawayTracker_SkipsSender_IfNewItemIsInactive()
    {
        var (sender, apiClient, tracker) = GenerateTracker(PrizeActive.id);

        apiClient.GetGiveaways(Arg.Any<CancellationToken>()).Returns(new[] { PrizeInactive });
        await tracker.UpdateAsync(CancellationToken.None);
        await sender.DidNotReceive().SendAsync(PrizeActive.title, PrizeActive.bid, PrizeActive.image);
    }
    
    [Test]
    public async Task GiveawayTracker_SkipsSender_IfNewItemHasNoBid()
    {
        var (sender, apiClient, tracker) = GenerateTracker(PrizeActive.id);

        apiClient.GetGiveaways(Arg.Any<CancellationToken>()).Returns(new[] { PrizeNoBid });
        await tracker.UpdateAsync(CancellationToken.None);
        await sender.DidNotReceive().SendAsync(PrizeActive.title, PrizeActive.bid, PrizeActive.image);
    }
    
    [Test]
    public async Task GiveawayTracker_UsesSender_IfNewItemIsActive()
    {
        var (sender, apiClient, tracker) = GenerateTracker(PrizeActive.id);
        
        apiClient.GetGiveaways(Arg.Any<CancellationToken>()).Returns(new[] { PrizeActive });
        await tracker.UpdateAsync(CancellationToken.None);
        await sender.Received(1).SendAsync(PrizeActive.title, PrizeActive.bid, PrizeActive.image);     
        
        await tracker.UpdateAsync(CancellationToken.None);
        await sender.Received(1).SendAsync(PrizeActive.title, PrizeActive.bid, PrizeActive.image);   
    }
    
    [Test]
    public async Task GiveawayTracker_UsesSender_IfExistingItemBecomesActive()
    {
        var (sender, apiClient, tracker) = GenerateTracker(PrizeActive.id);

        apiClient.GetGiveaways(Arg.Any<CancellationToken>()).Returns(new[] { PrizeInactive });
        await tracker.UpdateAsync(CancellationToken.None);
        await sender.DidNotReceive().SendAsync(PrizeActive.title, PrizeActive.bid, PrizeActive.image);
        
        apiClient.GetGiveaways(Arg.Any<CancellationToken>()).Returns(new[] { PrizeActive });
        await tracker.UpdateAsync(CancellationToken.None);
        await sender.Received(1).SendAsync(PrizeActive.title, PrizeActive.bid, PrizeActive.image);  
        
        await tracker.UpdateAsync(CancellationToken.None);
        await sender.Received(1).SendAsync(PrizeActive.title, PrizeActive.bid, PrizeActive.image);
    }
    
    [Test]
    public async Task GiveawayTracker_UsesSender_IfExistingItemGetsBid()
    {
        var (sender, apiClient, tracker) = GenerateTracker(PrizeActive.id);

        apiClient.GetGiveaways(Arg.Any<CancellationToken>()).Returns(new[] { PrizeNoBid });
        await tracker.UpdateAsync(CancellationToken.None);
        await sender.DidNotReceive().SendAsync(PrizeActive.title, PrizeActive.bid, PrizeActive.image);
        
        apiClient.GetGiveaways(Arg.Any<CancellationToken>()).Returns(new[] { PrizeActive });
        await tracker.UpdateAsync(CancellationToken.None);
        await sender.Received(1).SendAsync(PrizeActive.title, PrizeActive.bid, PrizeActive.image);  
        
        await tracker.UpdateAsync(CancellationToken.None);
        await sender.Received(1).SendAsync(PrizeActive.title, PrizeActive.bid, PrizeActive.image);
    }
    
    [Test]
    public async Task GiveawayTracker_UsesSender_IfNewItemGetsBid()
    {
        var (sender, apiClient, tracker) = GenerateTracker(PrizeActive.id);

        apiClient.GetGiveaways(Arg.Any<CancellationToken>()).Returns(new[] { PrizeInactive });
        await tracker.UpdateAsync(CancellationToken.None);
        await sender.DidNotReceive().SendAsync(PrizeActive.title, PrizeActive.bid, PrizeActive.image);

        apiClient.GetGiveaways(Arg.Any<CancellationToken>()).Returns(new[] { PrizeNoBid });
        await tracker.UpdateAsync(CancellationToken.None);
        await sender.DidNotReceive().SendAsync(PrizeActive.title, PrizeActive.bid, PrizeActive.image);
        
        apiClient.GetGiveaways(Arg.Any<CancellationToken>()).Returns(new[] { PrizeActive });
        await tracker.UpdateAsync(CancellationToken.None);
        await sender.Received(1).SendAsync(PrizeActive.title, PrizeActive.bid, PrizeActive.image);  
        
        await tracker.UpdateAsync(CancellationToken.None);
        await sender.Received(1).SendAsync(PrizeActive.title, PrizeActive.bid, PrizeActive.image);
    }
}