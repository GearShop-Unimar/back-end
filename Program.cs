using System.Text.Json;
using Microsoft.EntityFrameworkCore;
using Microsoft.OpenApi.Models;
using GearShop.Data;
using GearShop.Repositories;
using GearShop.Middleware;
using System.Text.Json.Serialization; // Necessário para JsonIgnoreCondition

var builder = WebApplication.CreateBuilder(args);

// =====================================================================
// 1. ADICIONAR O SERVIÇO CORS
// Define a política que permite a origem do seu frontend (Vite)
// =====================================================================
builder.Services.AddCors(options =>
{
    options.AddPolicy("CorsPolicyFrontend",
        builder => builder
            // Permite APENAS a origem do seu frontend (http://localhost:5173)
            .WithOrigins("http://localhost:5173")
            // Permite todos os métodos HTTP (GET, POST, PUT, DELETE)
            .AllowAnyMethod()
            // Permite todos os cabeçalhos HTTP
            .AllowAnyHeader());
});
// =====================================================================


builder.Services.AddControllers()
    .AddJsonOptions(o =>
    {
        o.JsonSerializerOptions.PropertyNamingPolicy = JsonNamingPolicy.CamelCase;
        o.JsonSerializerOptions.DefaultIgnoreCondition = JsonIgnoreCondition.WhenWritingNull;
    });

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(c =>
{
    c.SwaggerDoc("v1", new OpenApiInfo { Title = "GearShop API", Version = "v1" });
});

builder.Services.AddDbContext<AppDbContext>(opt =>
    opt.UseSqlite(builder.Configuration.GetConnectionString("Default")));

builder.Services.AddScoped<IUserRepository, EfUserRepository>();
builder.Services.AddScoped<IProductRepository, EfProductRepository>();

var app = builder.Build();

app.UseMiddleware<GlobalExceptionMiddleware>();

app.UseSwagger();
app.UseSwaggerUI(c => c.SwaggerEndpoint("/swagger/v1/swagger.json", "GearShop API v1"));


// =====================================================================
// 2. HABILITAR O MIDDLEWARE CORS
// Deve ser chamado antes de app.MapControllers()
// =====================================================================
app.UseCors("CorsPolicyFrontend");
// =====================================================================


app.UseHttpsRedirection();
app.MapControllers();

app.Run();