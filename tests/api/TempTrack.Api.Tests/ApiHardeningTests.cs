using System.Net;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Testing;
using TempTrack.Api.Infrastructure.Http;

namespace TempTrack.Api.Tests;

// None of these need a database: they never reach a query
public sealed class ApiHardeningTests
{
    private const string AllowedOrigin = "https://dev.temptrack.code-invention.com";

    private static WebApplicationFactory<Program> CreateFactory(string environment, string? connectionString = "Host=unused") =>
        new WebApplicationFactory<Program>().WithWebHostBuilder(b =>
        {
            b.UseEnvironment(environment);
            b.UseSetting("ConnectionStrings:Default", connectionString);
            b.UseSetting("Cors:AllowedOrigins:0", AllowedOrigin);
        });

    [Fact]
    public void Startup_fails_fast_when_connection_string_is_missing()
    {
        using var factory = CreateFactory("Production", connectionString: "");

        var ex = Assert.ThrowsAny<Exception>(() => factory.CreateClient());

        Assert.Contains("ConnectionStrings:Default", ex.ToString());
    }

    [Theory]
    [InlineData("Development", HttpStatusCode.OK)]
    [InlineData("Production", HttpStatusCode.NotFound)]
    public async Task OpenApi_document_is_served_only_in_development(string environment, HttpStatusCode expected)
    {
        await using var factory = CreateFactory(environment);
        var client = factory.CreateClient();

        var response = await client.GetAsync("/openapi/v1.json", TestContext.Current.CancellationToken);

        Assert.Equal(expected, response.StatusCode);
    }

    [Theory]
    [InlineData("has space")]
    [InlineData("line\r\nbreak")]
    [InlineData("aaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaa")]
    public async Task Unsafe_correlation_id_is_replaced_not_echoed(string unsafeId)
    {
        await using var factory = CreateFactory("Production");
        using var _ = factory.CreateClient();

        // HttpClient refuses CR/LF in header values, so set the raw header on the server request
        var context = await factory.Server.SendAsync(c =>
        {
            c.Request.Method = "GET";
            c.Request.Path = "/does-not-exist";
            c.Request.Headers[CorrelationIdMiddleware.HeaderName] = unsafeId;
        }, TestContext.Current.CancellationToken);

        var returned = context.Response.Headers[CorrelationIdMiddleware.HeaderName].Single();
        Assert.NotEqual(unsafeId, returned);
        Assert.Matches("^[0-9a-f]{32}$", returned);
    }

    [Fact]
    public async Task Cors_preflight_allows_configured_origin_and_exposes_correlation_id()
    {
        await using var factory = CreateFactory("Production");
        var client = factory.CreateClient();
        var request = new HttpRequestMessage(HttpMethod.Options, "/health");
        request.Headers.Add("Origin", AllowedOrigin);
        request.Headers.Add("Access-Control-Request-Method", "POST");
        request.Headers.Add("Access-Control-Request-Headers", "content-type, x-correlation-id");

        var response = await client.SendAsync(request, TestContext.Current.CancellationToken);

        Assert.Equal(HttpStatusCode.NoContent, response.StatusCode);
        Assert.Equal(AllowedOrigin, response.Headers.GetValues("Access-Control-Allow-Origin").Single());
        Assert.Contains("POST", response.Headers.GetValues("Access-Control-Allow-Methods").Single());
        Assert.Contains("X-Correlation-ID", response.Headers.GetValues("Access-Control-Allow-Headers").Single(),
            StringComparison.OrdinalIgnoreCase);
    }

    [Fact]
    public async Task Cors_does_not_allow_unlisted_origin_and_exposes_correlation_id_on_allowed_one()
    {
        await using var factory = CreateFactory("Production");
        var client = factory.CreateClient();
        var other = new HttpRequestMessage(HttpMethod.Get, "/does-not-exist");
        other.Headers.Add("Origin", "https://evil.example");
        var allowed = new HttpRequestMessage(HttpMethod.Get, "/does-not-exist");
        allowed.Headers.Add("Origin", AllowedOrigin);

        var otherResponse = await client.SendAsync(other, TestContext.Current.CancellationToken);
        var allowedResponse = await client.SendAsync(allowed, TestContext.Current.CancellationToken);

        Assert.False(otherResponse.Headers.Contains("Access-Control-Allow-Origin"));
        Assert.Contains("X-Correlation-ID", allowedResponse.Headers.GetValues("Access-Control-Expose-Headers").Single(),
            StringComparison.OrdinalIgnoreCase);
    }
}
