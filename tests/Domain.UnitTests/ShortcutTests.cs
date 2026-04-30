using FluentResults;
using LinkyFunky.Domain.Entities;
using LinkyFunky.Domain.Interfaces;

namespace Domain.UnitTests;

public class ShortcutTests
{
    [Fact]
    public void Create_WhenUrlIsValidAndGeneratorReturnsCode_ReturnsSuccess()
    {
        var userId = Guid.NewGuid();
        var longUrl = "https://example.com/path?q=1";
        var expectedCode = "AbC123";
        var shortCodeGen = new StubShortCodeGen(Result.Ok(expectedCode));

        var result = Shortcut.Create(userId, longUrl, shortCodeGen);

        Assert.True(result.IsSuccess);
        Assert.Equal(userId, result.Value.UserId);
        Assert.Equal(longUrl, result.Value.LongUrl);
        Assert.Equal(expectedCode, result.Value.ShortCode);
    }

    [Theory]
    [InlineData("")]
    [InlineData(" ")]
    [InlineData("ftp://example.com")]
    [InlineData("example.com")]
    public void Create_WhenUrlIsInvalid_ReturnsFailure(string longUrl)
    {
        var shortCodeGen = new StubShortCodeGen(Result.Ok("ABC123"));

        var result = Shortcut.Create(Guid.NewGuid(), longUrl, shortCodeGen);

        Assert.True(result.IsFailed);
        Assert.Contains(
            result.Errors,
            error => error.Message == "The long URL must be a valid absolute HTTP or HTTPS URL.");
    }

    [Fact]
    public void Create_WhenGeneratorReturnsFailure_ReturnsFailure()
    {
        var expectedMessage = "Generator failure.";
        var shortCodeGen = new StubShortCodeGen(Result.Fail(expectedMessage));

        var result = Shortcut.Create(Guid.NewGuid(), "https://example.com", shortCodeGen);

        Assert.True(result.IsFailed);
        Assert.Contains(result.Errors, error => error.Message == expectedMessage);
    }

    sealed class StubShortCodeGen(Result<string> result) : IShortCodeGen
    {
        public Result<string> Generate(string longUrl)
        {
            return result;
        }
    }
}
