var builder = DistributedApplication.CreateBuilder(args);

var linkyDb = builder
    .AddPostgres("linky-db")
    .WithDataVolume("linky-db_volume")
    .AddDatabase("linkyfunky");

var redis = builder
    .AddRedis("redis")
    .WithDataVolume("redis_volume");

var infraRootPath = Path.GetFullPath(
    Path.Combine(builder.AppHostDirectory, "..", "..", "infra"));

var prometheusRootPath = Path.Combine(infraRootPath, "prometheus");

var prometheus = builder
    .AddContainer("prometheus", "prom/prometheus:latest")
    .WithBindMount(prometheusRootPath, "/etc/prometheus")
    .WithEndpoint(9090, targetPort: 9090, name: "prometheus-ui");

var grafanaRootPath = Path.Combine(infraRootPath, "grafana");

builder
    .AddContainer("grafana", "grafana/grafana:latest")
    .WithBindMount(Path.Combine(grafanaRootPath, "config"), "/etc/grafana")
    .WithBindMount(Path.Combine(grafanaRootPath, "dashboards"), "/etc/grafana/dashboards")
    .WithHttpEndpoint(3000, targetPort: 3000, name: "grafana-ui")
    .WithEnvironment("GF_SECURITY_ADMIN_PASSWORD", "admin")
    .WaitFor(prometheus);

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
