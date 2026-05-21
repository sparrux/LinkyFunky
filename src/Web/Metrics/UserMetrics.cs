using System.Diagnostics.Metrics;

namespace Web.Metrics;

public sealed class UserMetrics
{
    public static readonly string InstrumentsSourceName = nameof(UserMetrics);

    public UserMetrics(IMeterFactory meterFactory)
    {
        var meter = meterFactory
            .Create(InstrumentsSourceName, "1.0.0");
        
        NewUserCounter = meter.CreateCounter<long>(
            name: "app.users_created",
            unit: "{user}",
            description: "Number of new users");
    }

    public Counter<long> NewUserCounter { get; }
}