using CommandService.Models;
using CommandService.SyncDataServices.Grpc;

namespace CommandService.Data;

public static class PrepDb
{
    public static void PrepPopulation(IApplicationBuilder applicationBuilder)
    {
        using var serviceScope = applicationBuilder.ApplicationServices.CreateScope();

        var grpcClient = serviceScope.ServiceProvider.GetService<IPlatformDataClient>();
        var repository = serviceScope.ServiceProvider.GetService<ICommandRepository>();

        var platforms = grpcClient.ReturnAllPlatforms();

        SeedData(repository, platforms);
    }

    private static void SeedData(ICommandRepository repository, IEnumerable<Platform> platforms)
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
}