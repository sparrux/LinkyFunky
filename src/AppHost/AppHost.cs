var builder = DistributedApplication.CreateBuilder(args);

var linkyDb = builder
    .AddPostgres("linky-db")
    .WithDataVolume("linky-db_volume")
    .AddDatabase("linkyfunky");

builder
    .AddProject<Projects.Web>("web")
    .WithReference(linkyDb)
    .WithExternalHttpEndpoints()
    .WaitFor(linkyDb);

builder.Build().Run();
