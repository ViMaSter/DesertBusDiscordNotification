using System.Net;
using Microsoft.AspNetCore.Mvc.Testing;

namespace DesertBusDiscordNotification.Test;

public class SmokeTests : WebApplicationFactory<Program>
{
    [Test]
    public void HealthReturns200()
    {
        var client = CreateClient();
        var response = client.GetAsync("/health").Result;
        Assert.AreEqual(HttpStatusCode.OK, response.StatusCode);
    }
}