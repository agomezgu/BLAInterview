using System.Net;
using Microsoft.AspNetCore.Mvc.Testing;

namespace BLAInterview.WebApi.UnitTests.Health;

public class HealthEndpointSpecs
{
    private const string HealthRoute = "/health";

    [Fact]
    public async Task HealthEndpoint_ReturnsSuccess_WhenApiIsRunning()
    {
        using var factory = new WebApplicationFactory<Program>();
        using var client = factory.CreateClient();

        var response = await client.GetAsync(HealthRoute);

        Assert.True(
            response.IsSuccessStatusCode,
            $"Expected a successful health response, but received {(int)response.StatusCode} {response.StatusCode}.");
    }
}
