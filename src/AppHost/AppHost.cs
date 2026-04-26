var builder = DistributedApplication.CreateBuilder(args);

var linkyDb = builder
    .AddPostgres("linky-db")
    .WithDataVolume()
    .AddDatabase("LinkyDb");

builder
    .AddProject<Projects.Web>("web")
    .WithReference(linkyDb)
    .WithExternalHttpEndpoints()
    .WaitFor(linkyDb);

builder.Build().Run();
