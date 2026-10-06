using System.Net;
using System.Net.Http.Json;
using TempTrack.Api.Infrastructure.Http;

namespace TempTrack.Api.Tests;

public sealed class HealthTests(ApiFactory factory) : IClassFixture<ApiFactory>
{
    [Fact]
    public async Task Health_returns_ok()
    {
        var client = factory.CreateClient();

        var response = await client.GetAsync("/health", TestContext.Current.CancellationToken);

        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        var body = await response.Content.ReadFromJsonAsync<HealthResponse>(TestContext.Current.CancellationToken);
        Assert.Equal("Up", body?.Database);
    }

    [Fact]
    public async Task Correlation_id_is_echoed_or_generated()
    {
        var client = factory.CreateClient();
        var request = new HttpRequestMessage(HttpMethod.Get, "/health");
        request.Headers.Add(CorrelationIdMiddleware.HeaderName, "abc-123");

        var echoed = await client.SendAsync(request, TestContext.Current.CancellationToken);
        var generated = await client.GetAsync("/health", TestContext.Current.CancellationToken);

        Assert.Equal("abc-123", echoed.Headers.GetValues(CorrelationIdMiddleware.HeaderName).Single());
        Assert.NotEmpty(generated.Headers.GetValues(CorrelationIdMiddleware.HeaderName).Single());
    }
}
