using Xunit;

/// <summary>
/// Web integration tests share heavy fixtures (Testcontainers, migrations); run sequentially to avoid migration races.
/// </summary>
[assembly: CollectionBehavior(DisableTestParallelization = true)]
