using AuthService.Data;
using AuthService.Extensions;
using AuthService.Services;
using Secxndary.MicroserviceApp.Shared;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddTransient<ITokenService, TokenService>();

builder.Services.ConfigureCors();
builder.Services.ConfigureJwtAuthentication();

builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

builder.Services.AddRouting(options => options.LowercaseUrls = true);

if (builder.Environment.IsProduction())
{
    builder.Services.ConfigureSqlServerDatabase(builder.Configuration);
}
else
{
    builder.Services.ConfigureInMemoryDatabase(builder.Configuration);
}

var app = builder.Build();


app.UseCors(GlobalConstants.CorsPolicy);

app.UseAuthentication();
app.UseAuthorization();

app.MapControllers();

PrepDb.PrepPopulation(app, builder.Environment.IsProduction());

app.Run();