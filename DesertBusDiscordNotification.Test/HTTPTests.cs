using System.Net;
using Microsoft.AspNetCore.Mvc.Testing;

namespace DesertBusDiscordNotification.Test;

public class SmokeTests : WebApplicationFactory<Program>
{
    private HttpClient _client = null!;

    [SetUp]
    public void Setup()
    {
        _client = CreateClient();
    }
    
    [Test]
    public void RootReturnsOK()
    {
        var response = _client.GetAsync("/").Result;
        Assert.Multiple(() =>
        {
            Assert.That(response.StatusCode, Is.EqualTo(HttpStatusCode.OK));
            Assert.That(response.Content.ReadAsStringAsync().Result, Is.EqualTo("OK"));
        });
    }

    [Test]
    public void VersionReturns200()
    {
        Assert.That(_client.GetAsync("/version").Result.StatusCode, Is.EqualTo(HttpStatusCode.OK));
    }
    
    [Test]
    public void HealthReturns200()
    {
        Assert.That(_client.GetAsync("/health").Result.StatusCode, Is.EqualTo(HttpStatusCode.OK));
    }
}