using CommandService.AsyncDataServices;
using CommandService.Data;
using CommandService.EventProcessing;
using CommandService.SyncDataServices.Grpc;
using Microsoft.EntityFrameworkCore;
using Secxndary.MicroserviceApp.Shared;

namespace CommandService.Extensions;

public static class ServiceExtensions
{
    public static void ConfigureRepository(this IServiceCollection services) => 
        services.AddScoped<ICommandRepository, CommandRepository>();

    public static void ConfigureSyncMessaging(this IServiceCollection services) => 
        services.AddScoped<IPlatformDataClient, PlatformDataClient>();

    public static void ConfigureAsyncMessaging(this IServiceCollection services)
    {
        services.AddHostedService<MessageBusSubscriber>();
        services.AddSingleton<IEventProcessor, EventProcessor>();
    }

    public static void ConfigureSqlServerDatabase(this IServiceCollection services, IConfiguration configuration)
    {
        var connectionString = configuration.GetConnectionString(GlobalConstants.CommandServiceConnectionStringName);
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