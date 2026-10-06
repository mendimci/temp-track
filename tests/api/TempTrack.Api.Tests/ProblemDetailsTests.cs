using System.Net;
using System.Text.Json;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.Extensions.DependencyInjection;
using TempTrack.Api.Infrastructure.Http;
using TempTrack.Api.Infrastructure.Persistence;

namespace TempTrack.Api.Tests;

public sealed class ProblemDetailsTests
{
    [Fact]
    public async Task Unhandled_exception_returns_problem_details_with_correlation_id_and_no_stack_trace()
    {
        // A DbContext that throws makes /health fail unhandled, without needing a database
        await using var factory = new WebApplicationFactory<Program>().WithWebHostBuilder(b =>
        {
            b.UseEnvironment("Production");
            b.UseSetting("ConnectionStrings:Default", "Host=unused");
            b.ConfigureServices(s => s.AddScoped<TempTrackDbContext>(_ => throw new InvalidOperationException("boom")));
        });
        var client = factory.CreateClient();

        var response = await client.GetAsync("/health", TestContext.Current.CancellationToken);

        Assert.Equal(HttpStatusCode.InternalServerError, response.StatusCode);
        var json = await response.Content.ReadAsStringAsync(TestContext.Current.CancellationToken);
        using var doc = JsonDocument.Parse(json);
        Assert.Equal(response.Headers.GetValues(CorrelationIdMiddleware.HeaderName).Single(),
            doc.RootElement.GetProperty("correlationId").GetString());
        Assert.DoesNotContain("InvalidOperationException", json);
        Assert.DoesNotContain("boom", json);
    }
}
