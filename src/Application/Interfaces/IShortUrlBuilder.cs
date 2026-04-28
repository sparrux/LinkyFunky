namespace LinkyFunky.Application.Interfaces;

public interface IShortUrlBuilder
{
    string Build(string shortCode);
}