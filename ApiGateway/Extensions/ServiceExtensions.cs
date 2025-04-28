using Ocelot.Cache.CacheManager;
using Ocelot.DependencyInjection;
using Secxndary.MicroserviceApp.Shared;

namespace ApiGateway.Extensions;

public static class ServiceExtensions
{
    public static void ConfigureOcelot(this IServiceCollection services, IConfiguration configuration, IWebHostEnvironment env)
    {
        var configBuilder = new ConfigurationBuilder()
            .SetBasePath(env.ContentRootPath)
            .AddJsonFile(GlobalConstants.OcelotConfigFileName, optional: false, reloadOnChange: true)
            .AddEnvironmentVariables();

        var updatedConfig = configBuilder.Build();

        services.AddOcelot(updatedConfig)
            .AddCacheManager(x => x.WithDictionaryHandle());
    } 
}