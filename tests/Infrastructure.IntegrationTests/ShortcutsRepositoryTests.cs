using FluentResults;
using Infrastructure.IntegrationTests.Fixtures;
using LinkyFunky.Domain.Entities;
using LinkyFunky.Domain.Interfaces;
using LinkyFunky.Infrastructure.Persistence.Repositories;
using Microsoft.EntityFrameworkCore;

namespace Infrastructure.IntegrationTests;

public sealed class ShortcutsRepositoryTests(PostgresDatabaseFixture fixture) : IClassFixture<PostgresDatabaseFixture>
{
    [Fact]
    public async Task UpdateRedirectsAsync_WhenShortcutsExist_UpdatesRedirectCounters()
    {
        var ctk = CancellationToken.None;
        await using var arrangeContext = fixture.CreateContext();
        var user = User.Create();
        await arrangeContext.Users.AddAsync(user, ctk);

        var firstShortcut = CreateShortcut(user.Id, "https://example.com/first", "abc123");
        var secondShortcut = CreateShortcut(user.Id, "https://example.com/second", "xyz789");

        await arrangeContext.Shortcuts.AddRangeAsync([firstShortcut, secondShortcut], ctk);
        await arrangeContext.SaveChangesAsync(ctk);

        await using var actContext = fixture.CreateContext();
        var repository = new ShortcutsRepository(actContext);

        var affectedRows = await repository.UpdateRedirectsAsync(
            new()
            {
                [firstShortcut.ShortCode] = 2,
                [secondShortcut.ShortCode] = 5
            },
            ctk);

        Assert.Equal(2, affectedRows);

        await using var assertContext = fixture.CreateContext();
        var updatedShortcuts = await assertContext.Shortcuts
            .OrderBy(x => x.ShortCode)
            .ToListAsync(ctk);

        Assert.Equal(2, updatedShortcuts.Count);
        Assert.Equal(2, updatedShortcuts[0].Redirects);
        Assert.Equal(5, updatedShortcuts[1].Redirects);
    }

    static Shortcut CreateShortcut(Guid userId, string longUrl, string shortCode)
    {
        var shortcutResult = Shortcut.Create(userId, longUrl, new StubShortCodeGen(Result.Ok(shortCode)));
        return shortcutResult.Value;
    }

    sealed class StubShortCodeGen(Result<string> result) : IShortCodeGen
    {
        public Result<string> Generate(string longUrl)
        {
            return result;
        }
    }
}
