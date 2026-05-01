using System.Net;
using System.Net.Http.Json;
using LinkyFunky.Application.Contracts.Requests;
using LinkyFunky.Application.Contracts.Responses;
using Microsoft.EntityFrameworkCore;

namespace Web.IntegrationTests;

public sealed class PostCreateShortcutEndpointTests(WebIntegrationTestFactory factory) : IClassFixture<WebIntegrationTestFactory>
{
    [Fact]
    public async Task PostCreateShortcutEndpoint_WhenRequestIsValid_ReturnsShortcutAndPersistsEntity()
    {
        var client = factory.CreateClient();
        var request = new CreateShortcutRequest
        {
            LongUrl = "https://example.com/articles/test"
        };

        var response = await client.PostAsJsonAsync("/shortcuts", request);

        Assert.Equal(HttpStatusCode.OK, response.StatusCode);

        var payload = await response.Content.ReadFromJsonAsync<ShortcutResponse>();
        Assert.NotNull(payload);
        Assert.False(string.IsNullOrWhiteSpace(payload.ShortCode));

        await using var dbContext = factory.CreateDbContext();
        var savedShortcut = await dbContext.Shortcuts
            .AsNoTracking()
            .SingleAsync(x => x.ShortCode == payload.ShortCode);

        Assert.Equal(request.LongUrl, savedShortcut.LongUrl);
        Assert.Equal(0, savedShortcut.Redirects);
    }
    
    [Theory]
    [InlineData(" ")]
    [InlineData("ftp://example.com")]
    [InlineData("example.com")]
    public async Task PostCreateShortcutEndpoint_WhenRequestIsInvalid_ReturnsErrorResponseList(string invalidUrl)
    {
        var client = factory.CreateClient();
        var request = new CreateShortcutRequest
        {
            LongUrl = invalidUrl
        };

        var response = await client.PostAsJsonAsync("/shortcuts", request);

        Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);

        var payload = await response.Content.ReadFromJsonAsync<ErrorResponseList>();
        Assert.NotNull(payload);
        Assert.Single(payload);
    }
}
