using System.Net;
using FluentResults;
using LinkyFunky.Domain.Entities;
using LinkyFunky.Domain.Interfaces;
using Microsoft.AspNetCore.Mvc.Testing;

namespace Web.IntegrationTests;

/// <summary>
/// Integration tests for GET /r/{shortCode} redirect behavior with seeded persistence.
/// </summary>
public sealed class GetRedirectByShortCodeEndpointTests(WebIntegrationTestFactory factory) : IClassFixture<WebIntegrationTestFactory>
{
    [Fact]
    public async Task GetRedirectByShortCodeEndpoint_WhenShortcutExistsInDatabase_ReturnsRedirectToLongUrl()
    {
        const string expectedLongUrl = "https://example.com/articles/seeded";
        const string shortCode = "seed01";

        await using (var arrangeContext = factory.CreateDbContext())
        {
            var user = User.Create();
            await arrangeContext.Users.AddAsync(user);

            var shortcutResult = Shortcut.Create(user.Id, expectedLongUrl, new StubShortCodeGen(Result.Ok(shortCode)));
            Assert.True(shortcutResult.IsSuccess);

            await arrangeContext.Shortcuts.AddAsync(shortcutResult.Value);
            await arrangeContext.SaveChangesAsync();
        }

        var client = factory.CreateClient(new() { AllowAutoRedirect = false });

        var response = await client.GetAsync($"/r/{shortCode}");

        Assert.Equal(HttpStatusCode.Redirect, response.StatusCode);
        Assert.Equal(expectedLongUrl, response.Headers.Location?.ToString());
    }

    /// <summary>
    /// Supplies a fixed short code so persisted shortcuts match predefined test data.
    /// </summary>
    sealed class StubShortCodeGen(Result<string> result) : IShortCodeGen
    {
        public Result<string> Generate(string longUrl) => result;
    }
}
