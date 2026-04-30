using LinkyFunky.Application;
using LinkyFunky.Infrastructure;
using LinkyFunky.ServiceDefaults;
using Scalar.AspNetCore;
using FastEndpoints;
using Web;
using Web.Authentication;
using Web.Middleware;

var builder = WebApplication.CreateBuilder(args);

builder.AddServiceDefaults();
builder.Services.AddApplication();
builder.Services.AddInfrastructure(builder.Configuration);
builder.Services.AddWebServices(builder.Configuration);

var app = builder.Build();

app.MapDefaultEndpoints();

if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
    app.MapScalarApiReference();
    await app.ApplyMigrationsAsync();
}

app.UseHttpsRedirection();
app.UseRouting();
app.UseAuthentication();
app.UseMiddleware<AnonymouslyAuthMiddleware>();
app.UseAuthorization();
app.UseMiddleware<UserDailyRateLimitMiddleware>();
app.UseFastEndpoints();

app.Run();