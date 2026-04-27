namespace Web.Options;

/// <summary>
/// Represents domain settings used to build public short URLs.
/// </summary>
public sealed class DomainOptions
{
    /// <summary>
    /// Configuration section name.
    /// </summary>
    public const string SectionName = "Domain";

    /// <summary>
    /// Gets or sets the base domain URL.
    /// </summary>
    public string BaseUrl { get; set; } = string.Empty;
}
