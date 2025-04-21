using AuthService;
using AuthService.Data;
using AuthService.Models;
using AuthService.Services;
using AuthService.Shared;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddTransient<ITokenService, TokenService>();

builder.Services.ConfigureCors();
builder.Services.ConfigureAuthentication();

builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

builder.Services.AddRouting(options => options.LowercaseUrls = true);

if (builder.Environment.IsProduction())
{
    var connectionString = builder.Configuration.GetConnectionString(Constants.ConnectionStringName);
    Console.WriteLine($"--> Using SQL Server Db\n--> Connection string: {connectionString}");
    builder.Services.AddDbContext<AppDbContext>(opts =>
        opts.UseSqlServer(connectionString));
}
else
{
    Console.WriteLine("--> Using InMemory Db");
    builder.Services.AddDbContext<AppDbContext>(opts =>
        opts.UseInMemoryDatabase(Constants.InMemoryDatabaseName));
}

var app = builder.Build();


app.UseCors(Constants.CorsPolicy);

app.UseAuthentication();
app.UseAuthorization();

app.MapControllers();

PrepDb.PrepPopulation(app, builder.Environment.IsProduction());

app.Run();