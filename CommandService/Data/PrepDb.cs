using CommandService.Models;
using CommandService.SyncDataServices.Grpc;
using Microsoft.EntityFrameworkCore;

namespace CommandService.Data;

public static class PrepDb
{
    private static bool _isProduction;

    public static void PrepPopulation(IApplicationBuilder applicationBuilder, bool isProduction)
    {
        _isProduction = isProduction;

        using var serviceScope = applicationBuilder.ApplicationServices.CreateScope();

        var context = serviceScope.ServiceProvider.GetService<AppDbContext>();
        var grpcClient = serviceScope.ServiceProvider.GetService<IPlatformDataClient>();
        var repository = serviceScope.ServiceProvider.GetService<ICommandRepository>();

        var platforms = grpcClient.ReturnAllPlatforms();

        SeedData(context, repository, platforms);
    }

    private static void SeedData(AppDbContext context, ICommandRepository repository, IEnumerable<Platform> platforms)
    {
        ApplyMigrations(context);

        SeedPlatforms(repository, platforms);
    }

    private static void SeedPlatforms(ICommandRepository repository, IEnumerable<Platform> platforms)
    {
        Console.WriteLine("--> Seeding new platforms...");

        var addedPlatformsCount = 0;
        var platformsList = platforms.ToList();

        foreach (var platform in platformsList)
        {
            if (!repository.ExternalPlatformExists(platform.ExternalId))
            {
                repository.CreatePlatform(platform);
                addedPlatformsCount++;
            }

            repository.SaveChanges();
        }

        Console.WriteLine($"--> Got {platformsList.Count} platforms from PlatformService, added {addedPlatformsCount} platforms to CommandService");
    }

    private static void ApplyMigrations(AppDbContext context)
    {
        if (!_isProduction)
        {
            return;
        }

        Console.WriteLine("--> Attempting to apply migrations...");

        try
        {
            context.Database.Migrate();            
        }
        catch (Exception ex)
        {
            Console.WriteLine($"--> Could not run migrations: {ex.Message}");
        }
    }
}