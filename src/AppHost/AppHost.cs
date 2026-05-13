var builder = DistributedApplication.CreateBuilder(args);

var linkyDb = builder
    .AddPostgres("linky-db")
    .WithDataVolume("linky-db_volume")
    .AddDatabase("linkyfunky");

var redis = builder
    .AddRedis("redis")
    .WithDataVolume("redis_volume");

var web = builder
    .AddProject<Projects.Web>("web")
    .WithReference(linkyDb)
    .WithReference(redis)
    .WithExternalHttpEndpoints()
    .WaitFor(linkyDb)
    .WaitFor(redis);

var loadTestsScriptsPath = Path.GetFullPath(
    Path.Combine(builder.AppHostDirectory, "..", "..", "tests", "Web.LoadTests", "scripts"));

builder
    .AddK6("k6")
    .WithLifetime(ContainerLifetime.Session)
    .WithImagePullPolicy(ImagePullPolicy.Always)
    .WithBindMount(loadTestsScriptsPath, "/scripts", true)
    .WithScript("/scripts/main.js")
    .WithReference(web)
    .WaitFor(web);

builder.Build().Run();
