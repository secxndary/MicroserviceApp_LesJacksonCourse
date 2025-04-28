using AuthService.Data;
using Microsoft.EntityFrameworkCore;
using Secxndary.MicroserviceApp.Shared;

namespace AuthService.Extensions;

public static class ServiceExtensions
{
    public static void ConfigureSqlServerDatabase(this IServiceCollection services, IConfiguration configuration)
    {
        var connectionString = configuration.GetConnectionString(GlobalConstants.AuthServiceConnectionStringName);
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