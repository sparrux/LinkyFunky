namespace LinkyFunky.Application.Contracts.Services;

public interface IShortUrlBuilder
{
    string Build(string shortCode);
}