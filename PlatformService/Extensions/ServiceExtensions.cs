using Microsoft.EntityFrameworkCore;
using PlatformService.AsyncDataServices;
using PlatformService.Data;
using PlatformService.SyncDataServices.Http;
using Secxndary.MicroserviceApp.Shared;

namespace PlatformService.Extensions;

public static class ServiceExtensions
{
    public static void ConfigureRepository(this IServiceCollection services) => 
        services.AddScoped<IPlatformRepository, PlatformRepository>();

    public static void ConfigureSyncMessaging(this IServiceCollection services) => 
        services.AddHttpClient<ICommandDataClient, HttpCommandDataClient>();

    public static void ConfigureAsyncMessaging(this IServiceCollection services) => 
        services.AddSingleton<IMessageBusClient, MessageBusClient>();

    public static void ConfigureGrpc(this IServiceCollection services) => 
        services.AddGrpc();

    public static void ConfigureSqlServerDatabase(this IServiceCollection services, IConfiguration configuration)
    {
        var connectionString = configuration.GetConnectionString(GlobalConstants.PlatformServiceConnectionStringName);
        Console.WriteLine($"--> Using SQL Server Db\n--> Connection string: {connectionString}");

        services.AddDbContext<AppDbContext>(opts =>
            opts.UseSqlServer(connectionString));
    }

    public static void ConfigureInMemoryDatabase(this IServiceCollection services, IConfiguration configuration)
    {
        Console.WriteLine("--> Using InMem Db");

        services.AddDbContext<AppDbContext>(opts => 
            opts.UseInMemoryDatabase(GlobalConstants.InMemoryDatabaseName));
    }
}