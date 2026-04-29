namespace LinkyFunky.Application.Interfaces.Cache;

public interface ICacheable
{
    TimeSpan? Expiry { get; }
}