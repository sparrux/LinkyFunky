using FastEndpoints;

namespace Web.Endpoints.HealthCheck;

/// <summary>
/// Handles health check requests and returns service availability state.
/// </summary>
public class GetHealthCheckEndpoint : EndpointWithoutRequest
{
    public override void Configure()
    {
        Get("/health");
        AllowAnonymous();
    }

    public override Task HandleAsync(CancellationToken ctk)
    {
        return HttpContext.Response.SendStatusCodeAsync(StatusCodes.Status200OK, ctk);
    }
}
