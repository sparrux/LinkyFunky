using LinkyFunky.Infrastructure.Services;

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

        Assert.Equal(firstCode, secondCode);
    }

    [Fact]
    public void Generate_ShouldReturnCodeWithFixedLength()
    {
        var sut = CreateSut();

        var code = sut.Generate("https://example.com");

        Assert.Equal(DefaultCodeLength, code.Length);
    }

    [Fact]
    public void Generate_ShouldReturnOnlyBase62Characters()
    {
        var sut = CreateSut();

        var code = sut.Generate("https://example.com/base62");

        Assert.Matches($"^[0-9A-Za-z]{{{DefaultCodeLength}}}$", code);
    }

    [Fact]
    public void Generate_ShouldIgnoreLeadingAndTrailingWhitespace()
    {
        var sut = CreateSut();
        var cleanUrl = "https://example.com/trim";
        var paddedUrl = "   https://example.com/trim   ";

        var cleanCode = sut.Generate(cleanUrl);
        var paddedCode = sut.Generate(paddedUrl);

        Assert.Equal(cleanCode, paddedCode);
    }

    [Theory]
    [InlineData("")]
    [InlineData(" ")]
    [InlineData("   ")]
    public void Generate_ShouldThrowArgumentException_WhenUrlIsEmptyOrWhitespace(string longUrl)
    {
        var sut = CreateSut();

        var exception = Assert.Throws<ArgumentException>(() => sut.Generate(longUrl));

        Assert.Equal("longUrl", exception.ParamName);
    }

    [Fact]
    public void Generate_ShouldThrowArgumentException_WhenUrlIsNull()
    {
        var sut = CreateSut();

        var exception = Assert.Throws<ArgumentException>(() => sut.Generate(null!));

        Assert.Equal("longUrl", exception.ParamName);
    }

    [Fact]
    public void Generate_ShouldUseConfiguredLength()
    {
        var codeLength = 12;
        var sut = CreateSut(codeLength);

        var code = sut.Generate("https://example.com/custom-length");

        Assert.Equal(codeLength, code.Length);
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
