namespace LinkyFunky.Application.Defaults;

public static class CacheDefaults
{
    public static TimeSpan ShortcutsExpiry => TimeSpan.FromHours(12);
    
    public static string LongUrlKey(string shortCode) => "long-url:" + shortCode;
}