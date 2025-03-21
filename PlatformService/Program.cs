using Microsoft.EntityFrameworkCore;
using PlatformService.Data;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddScoped<IPlatformRepository, PlatformRepository>();

builder.Services.AddDbContext<AppDbContext>(opts => 
    opts.UseInMemoryDatabase("InMem"));

builder.Services.AddControllers();

// TODO maybe change to Assembly.GetExecutingAssembly() ?
builder.Services.AddAutoMapper(AppDomain.CurrentDomain.GetAssemblies());

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var app = builder.Build();


if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();
app.UseAuthorization();
app.MapControllers();

PrepDb.PrepPopulation(app);

app.Run();