using CommandService.Data;
using CommandService.Extensions;
using Secxndary.MicroserviceApp.Shared;

var builder = WebApplication.CreateBuilder(args);

builder.Services.ConfigureRepository();

if (builder.Environment.IsProduction())
{
    builder.Services.ConfigureSqlServerDatabase(builder.Configuration);
}
else
{
    builder.Services.ConfigureInMemoryDatabase(builder.Configuration);
}

builder.Services.AddControllers();

builder.Services.ConfigureSyncMessaging();
builder.Services.ConfigureAsyncMessaging();

builder.Services.ConfigureAutoMapper();

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var app = builder.Build();


if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseAuthorization();
app.MapControllers();

PrepDb.PrepPopulation(app);

app.Run();