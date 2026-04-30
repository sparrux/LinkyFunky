namespace LinkyFunky.Application.Interfaces;

/// <summary>
/// Defines which per-user daily quota applies to the current HTTP operation.
/// </summary>
public enum UserRateLimitKind
{
    /// <summary>
    /// Creating a new shortcut (POST create shortcut).
    /// </summary>
    CreateShortcut,

    /// <summary>
    /// Following a short link redirect (GET by short code).
    /// </summary>
    RedirectShortcut,
}

/// <summary>
/// Result of attempting to consume one unit from a user's daily rate limit.
/// </summary>
/// <param name="Allowed">Whether the request may proceed.</param>
/// <param name="Limit">Configured maximum number of operations per UTC day.</param>
/// <param name="Remaining">Remaining quota after a successful acquire; zero when denied.</param>
/// <param name="RetryAfter">When denied, time until the next UTC day window resets quota.</param>
public sealed record RateLimitAcquireResult(
    bool Allowed,
    int Limit,
    int Remaining,
    TimeSpan? RetryAfter);

/// <summary>
/// Enforces per-user daily quotas stored in a distributed store (e.g. Redis).
/// </summary>
public interface IUserDailyRateLimitService
{
    /// <summary>
    /// Atomically tries to consume one slot for the user in the current UTC calendar day.
    /// </summary>
    /// <param name="userId">Authenticated user identifier.</param>
    /// <param name="kind">Quota bucket to consume.</param>
    /// <param name="ctk">Cancellation token.</param>
    /// <returns>Whether the operation is allowed and header-friendly quota metadata.</returns>
    Task<RateLimitAcquireResult> TryAcquireAsync(Guid userId, UserRateLimitKind kind, CancellationToken ctk);
}
