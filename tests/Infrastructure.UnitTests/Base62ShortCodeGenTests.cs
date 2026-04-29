using LinkyFunky.Infrastructure.Services.ShortCodeGen;

namespace Infrastructure.UnitTests;

public class Base62ShortCodeGenTests
{
    const int DefaultCodeLength = 5;

    [Fact]
    public void Generate_ShouldReturnDeterministicCode_ForSameUrl()
    {
        var sut = CreateSut();
        var longUrl = "https://example.com/some/path?q=1";

        var firstCode = sut.Generate(longUrl);
        var secondCode = sut.Generate(longUrl);

        Assert.True(firstCode.IsSuccess);
        Assert.True(secondCode.IsSuccess);
        Assert.Equal(firstCode.Value, secondCode.Value);
    }

    [Fact]
    public void Generate_ShouldReturnCodeWithFixedLength()
    {
        var sut = CreateSut();

        var code = sut.Generate("https://example.com");

        Assert.True(code.IsSuccess);
        Assert.Equal(DefaultCodeLength, code.Value.Length);
    }

    [Fact]
    public void Generate_ShouldReturnOnlyBase62Characters()
    {
        var sut = CreateSut();

        var code = sut.Generate("https://example.com/base62");

        Assert.True(code.IsSuccess);
        Assert.Matches($"^[0-9A-Za-z]{{{DefaultCodeLength}}}$", code.Value);
    }

    [Fact]
    public void Generate_ShouldIgnoreLeadingAndTrailingWhitespace()
    {
        var sut = CreateSut();
        var cleanUrl = "https://example.com/trim";
        var paddedUrl = "   https://example.com/trim   ";

        var cleanCode = sut.Generate(cleanUrl);
        var paddedCode = sut.Generate(paddedUrl);

        Assert.True(cleanCode.IsSuccess);
        Assert.True(paddedCode.IsSuccess);
        Assert.Equal(cleanCode.Value, paddedCode.Value);
    }

    [Theory]
    [InlineData("")]
    [InlineData(" ")]
    [InlineData("   ")]
    public void Generate_ShouldReturnFailure_WhenUrlIsEmptyOrWhitespace(string longUrl)
    {
        var sut = CreateSut();

        var result = sut.Generate(longUrl);

        Assert.True(result.IsFailed);
        Assert.Contains(result.Errors, error => error.Message == "Long URL must not be null or empty.");
    }

    [Fact]
    public void Generate_ShouldReturnFailure_WhenUrlIsNull()
    {
        var sut = CreateSut();

        var result = sut.Generate(null!);

        Assert.True(result.IsFailed);
        Assert.Contains(result.Errors, error => error.Message == "Long URL must not be null or empty.");
    }

    [Fact]
    public void Generate_ShouldUseConfiguredLength()
    {
        var codeLength = 12;
        var sut = CreateSut(codeLength);

        var code = sut.Generate("https://example.com/custom-length");

        Assert.True(code.IsSuccess);
        Assert.Equal(codeLength, code.Value.Length);
    }

    [Theory]
    [InlineData(0)]
    [InlineData(-1)]
    public void Constructor_ShouldThrowArgumentOutOfRangeException_WhenCodeLengthIsNotPositive(int codeLength)
    {
        var exception = Assert.Throws<ArgumentOutOfRangeException>(() => CreateSut(codeLength));

        Assert.Equal("codeLength", exception.ParamName);
    }

    static Base62ShortCodeGen CreateSut(int codeLength = DefaultCodeLength)
    {
        return new(codeLength);
    }
}
