using Ocelot.Cache.CacheManager;
using Ocelot.DependencyInjection;
using Ocelot.Middleware;
using Shared;

var builder = WebApplication.CreateBuilder(args);
var isProd = builder.Environment.IsProduction(); 

builder.Services.ConfigureJwtAuthentication();

if (isProd)
{
    builder.Configuration.AddJsonFile("ocelot.json", optional: false, reloadOnChange: true);
    builder.Services.AddOcelot(builder.Configuration)
        .AddCacheManager(x => x.WithDictionaryHandle());
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