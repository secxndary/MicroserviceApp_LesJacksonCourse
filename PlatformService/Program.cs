using Microsoft.EntityFrameworkCore;
using PlatformService.Data;
using PlatformService.SyncDataServices.Http;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddScoped<IPlatformRepository, PlatformRepository>();
builder.Services.AddHttpClient<ICommandDataClient, HttpCommandDataClient>();

if (builder.Environment.IsProduction())
{
    var connectionString = builder.Configuration.GetConnectionString("PlatformsConnection");
    Console.WriteLine($"--> Using SQL Server Db\n--> Connection string: {connectionString}");
    builder.Services.AddDbContext<AppDbContext>(opts =>
        opts.UseSqlServer(connectionString));
}
else
{
    Console.WriteLine($"--> Using InMem Db");
    builder.Services.AddDbContext<AppDbContext>(opts => 
        opts.UseInMemoryDatabase("InMem"));
}

builder.Services.AddControllers();

// TODO maybe change to Assembly.GetExecutingAssembly() ?
builder.Services.AddAutoMapper(AppDomain.CurrentDomain.GetAssemblies());

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

Console.WriteLine($"--> CommandService Endpoint: {builder.Configuration["CommandServiceUrl"]}");

var app = builder.Build();


if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseAuthorization();
app.MapControllers();

PrepDb.PrepPopulation(app, builder.Environment.IsProduction());

app.Run();