using FastEndpoints;
using LinkyFunky.Application;
using LinkyFunky.Infrastructure;
using LinkyFunky.ServiceDefaults;
using Scalar.AspNetCore;
using Web;
using Web.Middlewares;

var builder = WebApplication.CreateBuilder(args);

builder.AddServiceDefaults();
builder.Services.AddApplication();
builder.Services.AddInfrastructure(builder.Configuration);
builder.AddWebServices(builder.Configuration);

var app = builder.Build();

app.MapDefaultEndpoints();

if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
    app.MapScalarApiReference();
    await app.ApplyMigrationsAsync();
}

app.UseHttpsRedirection();
app.UseAuthentication();
app.UseMiddleware<GuestMiddleware>();
app.UseAuthorization();
app.UseMiddleware<UserDailyRateLimitMiddleware>();
app.UseFastEndpoints();

app.Run();

public partial class Program;