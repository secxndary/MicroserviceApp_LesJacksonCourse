using ApiGateway.Extensions;
using Ocelot.Middleware;
using Secxndary.MicroserviceApp.Shared;

var builder = WebApplication.CreateBuilder(args);
var isProd = builder.Environment.IsProduction(); 

builder.Services.ConfigureJwtAuthentication();

if (isProd)
{
    builder.Services.ConfigureOcelot(builder.Configuration, builder.Environment);
}

builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();

var app = builder.Build();


app.UseHttpsRedirection();
app.UseAuthorization();
app.MapControllers();

app.MapGet("/health", () => "Api Gateway is working correctly");

if (isProd) await app.UseOcelot();

app.Run();