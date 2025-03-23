using Microsoft.EntityFrameworkCore;
using PlatformService.Models;

namespace PlatformService.Data;

public static class PrepDb
{
    private static bool _isProduction;

    public static void PrepPopulation(IApplicationBuilder app, bool isProduction)
    {
        _isProduction = isProduction;

        using var serviceScope = app.ApplicationServices.CreateScope();

        SeedData(serviceScope.ServiceProvider.GetService<AppDbContext>());
    }

    private static void SeedData(AppDbContext context)
    {
        if (_isProduction)
        {
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

        if (context.Platforms.Any())
        {
            Console.WriteLine("--> We already have data");
            return;
        }

        Console.WriteLine("--> Seeding data...");

        context.Platforms.AddRange(
            new Platform { Name = ".NET", Publisher = "Microsoft", Cost = "Free" },
            new Platform { Name = "SQL Server Express", Publisher = "Microsoft", Cost = "Free" },
            new Platform { Name = "Kubernetes", Publisher = "Cloud Native Computing Foundation", Cost = "Free" }
        );

        context.SaveChanges();
    }
}