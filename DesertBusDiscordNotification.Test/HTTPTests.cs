using System.Net;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.Extensions.DependencyInjection;

namespace DesertBusDiscordNotification.Test;

public class SmokeTests : WebApplicationFactory<Program>
{

    protected override void ConfigureWebHost(IWebHostBuilder builder)
    {
        builder.ConfigureServices(services =>
        {
            // set mock password for hangfire dashboard
            services.Configure<DesertBusDiscordNotification.Options.Hangfire>(options => options.Password = "password");
        });
    }

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