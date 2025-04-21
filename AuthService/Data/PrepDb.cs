using AuthService.Models;
using Microsoft.CodeAnalysis;
using Microsoft.EntityFrameworkCore;
namespace AuthService.Data;

public static class PrepDb
{
    private static bool _isProduction;

    public static void PrepPopulation(IApplicationBuilder app, bool isProduction)
    {
        _isProduction = isProduction;

        using var serviceScope = app.ApplicationServices.CreateScope();

        SeedData(serviceScope.ServiceProvider.GetService<AppDbContext>()!);
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

        if (context.LoginModels.Any())
        {
            Console.WriteLine($"--> We already have data: {context.LoginModels.Count()} rows in the database");
            return;
        }

        Console.WriteLine("--> Seeding data...");

        context.LoginModels.AddRange(
            new LoginModel { UserName = "user", Password = "qweqwe" },
            new LoginModel { UserName = "johndoe", Password = "def@123" }
        );

        context.SaveChanges();

        Console.WriteLine($"--> Successfully added {context.LoginModels.Count()} entities");
    }
}