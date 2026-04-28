var builder = DistributedApplication.CreateBuilder(args);

var linkyDb = builder
    .AddPostgres("linky-db")
    .WithDataVolume("linky-db_volume")
    .AddDatabase("linkyfunky");

var redis = builder
    .AddRedis("redis")
    .WithDataVolume("redis_volume");

builder
    .AddProject<Projects.Web>("web")
    .WithReference(linkyDb)
    .WithReference(redis)
    .WithExternalHttpEndpoints()
    .WaitFor(linkyDb)
    .WaitFor(redis);

builder.Build().Run();
