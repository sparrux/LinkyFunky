using LinkyFunky.Infrastructure.Options;
using LinkyFunky.Infrastructure.Services.Builders;
using Microsoft.Extensions.Options;

namespace Infrastructure.UnitTests;

public class ShortUrlBuilderTests
{
    [Fact]
    public void Constructor_ShouldThrowArgumentException_WhenBaseUrlIsInvalid()
    {
        var options = CreateDomainOptions("not-a-valid-url");

        var exception = Assert.Throws<ArgumentException>(() => new ShortUrlBuilder(options));

        Assert.Equal("domainOptions", exception.ParamName);
        Assert.Equal("Domain:BaseUrl must be a valid absolute URL. (Parameter 'domainOptions')", exception.Message);
    }

    [Theory]
    [InlineData("https://linkyfunky.com", "abc123", "https://linkyfunky.com/abc123")]
    [InlineData("https://linkyfunky.com/", "abc123", "https://linkyfunky.com/abc123")]
    [InlineData("https://linkyfunky.com", "/abc123", "https://linkyfunky.com/abc123")]
    [InlineData("https://linkyfunky.com/", "/abc123", "https://linkyfunky.com/abc123")]
    [InlineData("https://linkyfunky.com", "///abc123", "https://linkyfunky.com/abc123")]
    public void Build_ShouldReturnExpectedShortUrl(string baseUrl, string shortCode, string expectedShortUrl)
    {
        var sut = CreateSut(baseUrl);

        var shortUrl = sut.Build(shortCode);

        Assert.Equal(expectedShortUrl, shortUrl);
    }

    static ShortUrlBuilder CreateSut(string baseUrl)
    {
        var domainOptions = CreateDomainOptions(baseUrl);
        return new(domainOptions);
    }

    static IOptions<DomainOptions> CreateDomainOptions(string baseUrl)
    {
        return Microsoft.Extensions.Options.Options.Create(new DomainOptions
        {
            BaseUrl = baseUrl
        });
    }
}
