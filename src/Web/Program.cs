using LinkyFunky.Application;
using LinkyFunky.Infrastructure;
using LinkyFunky.ServiceDefaults;
using LinkyFunky.Web;
using LinkyFunky.Web.Authentication;
using Scalar.AspNetCore;
using FastEndpoints;

var builder = WebApplication.CreateBuilder(args);

builder.AddServiceDefaults();
builder.Services.AddApplication();
builder.Services.AddInfrastructure(builder.Configuration);
builder.Services.AddWebServices();

builder.Services.AddOpenApi();
builder.Services.AddFastEndpoints();

var app = builder.Build();

app.MapDefaultEndpoints();

if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
    app.MapScalarApiReference();
}

app.UseHttpsRedirection();
app.UseRouting();
app.UseAuthentication();
app.UseMiddleware<AnonymouslyAuthMiddleware>();
app.UseAuthorization();
app.UseFastEndpoints();

app.Run();