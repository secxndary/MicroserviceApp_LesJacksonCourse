using PlatformService.Data;
using PlatformService.Extensions;
using PlatformService.SyncDataServices.Grpc;
using Secxndary.MicroserviceApp.Shared;

var builder = WebApplication.CreateBuilder(args);

builder.Services.ConfigureRepository();
builder.Services.ConfigureSyncMessaging();
builder.Services.ConfigureAsyncMessaging();
builder.Services.ConfigureGrpc();

if (builder.Environment.IsProduction())
{
    builder.Services.ConfigureSqlServerDatabase(builder.Configuration);
}
else
{
    builder.Services.ConfigureInMemoryDatabase(builder.Configuration);
}

builder.Services.AddControllers();

builder.Services.ConfigureAutoMapper();

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

app.MapGrpcService<GrpcPlatformService>();
app.MapGet("/protos/platforms.proto", async context =>
{
    await context.Response.WriteAsync(File.ReadAllText("Protos/platforms.proto"));
});

PrepDb.PrepPopulation(app, builder.Environment.IsProduction());

app.Run();