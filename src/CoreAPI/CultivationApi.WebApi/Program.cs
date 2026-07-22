using CultivationApi.WebApi.Middleware;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddControllers();
builder.Services.AddProblemDetails();
builder.Services.AddExceptionHandler<GlobalExceptionHandler>();

builder.Services.AddDbContext<CultivationApi.Infrastructure.Data.AppDbContext>(options =>
    options.UseNpgsql(builder.Configuration.GetConnectionString("DefaultConnection")));

builder.Services.AddScoped<CultivationApi.Application.Interfaces.IAppDbContext>(provider => provider.GetRequiredService<CultivationApi.Infrastructure.Data.AppDbContext>());
builder.Services.AddScoped<CultivationApi.Application.Services.ICharacterService, CultivationApi.Application.Services.CharacterService>();

var app = builder.Build();

// Configure the HTTP request pipeline.
app.UseExceptionHandler();

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

app.Run();
