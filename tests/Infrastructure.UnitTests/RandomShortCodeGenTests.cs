using LinkyFunky.Infrastructure.Services.ShortCodeGen;

namespace Infrastructure.UnitTests;

public class RandomShortCodeGenTests
{
    const int DefaultCodeLength = 8;

    [Fact]
    public void Generate_WhenLongUrlIsValid_ReturnsCodeWithFixedLength()
    {
        var sut = CreateSut();

        var result = sut.Generate("https://example.com");

        Assert.True(result.IsSuccess);
        Assert.Equal(DefaultCodeLength, result.Value.Length);
    }

    [Fact]
    public void Generate_WhenLongUrlIsValid_ReturnsOnlyBase62Characters()
    {
        var sut = CreateSut();

        var result = sut.Generate("https://example.com/base62");

        Assert.True(result.IsSuccess);
        Assert.Matches($"^[0-9A-Za-z]{{{DefaultCodeLength}}}$", result.Value);
    }

    [Theory]
    [InlineData("")]
    [InlineData(" ")]
    [InlineData("   ")]
    public void Generate_WhenLongUrlIsEmptyOrWhitespace_ReturnsFailure(string longUrl)
    {
        var sut = CreateSut();

        var result = sut.Generate(longUrl);

        Assert.True(result.IsFailed);
        Assert.Contains(result.Errors, error => error.Message == "Long URL must not be null or empty.");
    }

    [Fact]
    public void Generate_WhenLongUrlIsNull_ReturnsFailure()
    {
        var sut = CreateSut();

        var result = sut.Generate(null!);

        Assert.True(result.IsFailed);
        Assert.Contains(result.Errors, error => error.Message == "Long URL must not be null or empty.");
    }

    [Fact]
    public void Generate_WhenCodeLengthIsConfigured_ReturnsCodeWithConfiguredLength()
    {
        var codeLength = 12;
        var sut = CreateSut(codeLength);

        var result = sut.Generate("https://example.com/custom-length");

        Assert.True(result.IsSuccess);
        Assert.Equal(codeLength, result.Value.Length);
    }

    [Theory]
    [InlineData(0)]
    [InlineData(-1)]
    public void Constructor_WhenCodeLengthIsNotPositive_ThrowsArgumentOutOfRangeException(int codeLength)
    {
        var exception = Assert.Throws<ArgumentOutOfRangeException>(() => CreateSut(codeLength));

        Assert.Equal("codeLength", exception.ParamName);
    }

    static RandomShortCodeGen CreateSut(int codeLength = DefaultCodeLength)
    {
        return new(codeLength);
    }
}
