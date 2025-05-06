using Microsoft.EntityFrameworkCore;
using PlatformService.Models;

namespace PlatformService.Data;

public static class PrepDb
{
    private static bool _isProduction;

    public static void PrepPopulation(IApplicationBuilder applicationBuilder, bool isProduction)
    {
        _isProduction = isProduction;

        using var serviceScope = applicationBuilder.ApplicationServices.CreateScope();

        SeedData(serviceScope.ServiceProvider.GetService<AppDbContext>());
    }

    private static void SeedData(AppDbContext context)
    {
        ApplyMigrations(context);

        if (!DataExistsInDatabase(context))
        {
            SeedPlatforms(context);
        }
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

    private static void SeedPlatforms(AppDbContext context)
    {
        Console.WriteLine("--> Seeding data...");

        context.Platforms.AddRange(
            new Platform { Name = ".NET", Publisher = "Microsoft", Cost = "Free" },
            new Platform { Name = "SQL Server Express", Publisher = "Microsoft", Cost = "Free" },
            new Platform { Name = "Kubernetes", Publisher = "Cloud Native Computing Foundation", Cost = "Free" }
        );

        context.SaveChanges();
    }

    private static bool DataExistsInDatabase(AppDbContext context)
    {
        if (!context.Platforms.Any())
        {
            return false;
        } 

        Console.WriteLine("--> We already have data");

        return true;
    }
}