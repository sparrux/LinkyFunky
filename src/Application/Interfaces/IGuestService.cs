namespace LinkyFunky.Application.Interfaces;

/// <summary>
/// Provides guest information of current HTTP request.
/// </summary>
public interface IGuestService
{
    /// <summary>
    /// User current HTTP request provides guest info.
    /// </summary>
    bool IsGuest { get; }
    
    /// <summary>
    /// Gets current guest id or default.
    /// </summary>
    Guid GuestIdOrDefault { get; }

    /// <summary>
    /// Set guest identification to current HTTP request.
    /// </summary>
    void SetGuest();
    
    /// <summary>
    /// Removes current guest information.
    /// </summary>
    void DeleteGuest();
}