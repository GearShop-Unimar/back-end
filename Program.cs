using System.Text.Json;
using Microsoft.EntityFrameworkCore;
using Microsoft.OpenApi.Models;
using GearShop.Data;
using GearShop.Repositories;
using GearShop.Middleware;
using System.Text.Json.Serialization;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using System.Text;
using GearShop.Services;
using GearShop.Services.User;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddCors(options =>
{
    options.AddPolicy("CorsPolicyFrontend",
        builder => builder
            .WithOrigins("http://localhost:5173")
            .AllowAnyMethod()
            .AllowAnyHeader());
});

// =====================================================================
// 🟢 CONFIGURAÇÃO JWT
// =====================================================================
var key = Encoding.ASCII.GetBytes(builder.Configuration["Jwt:Key"]!); // A chave deve vir de appsettings

builder.Services.AddAuthentication(x =>
{
    x.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
    x.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
})
.AddJwtBearer(x =>
{
    x.RequireHttpsMetadata = false;
    x.SaveToken = true;
    x.TokenValidationParameters = new TokenValidationParameters
    {
        ValidateIssuerSigningKey = true,
        IssuerSigningKey = new SymmetricSecurityKey(key),
        ValidateIssuer = false, // Em desenvolvimento, podemos desabilitar
        ValidateAudience = false // Em desenvolvimento, podemos desabilitar
    };
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

    // 🟢 Adiciona a opção de Token JWT no Swagger UI
    var securityScheme = new OpenApiSecurityScheme
    {
        Name = "Authorization",
        Description = "Insira o token JWT no formato: Bearer SEU_TOKEN",
        In = ParameterLocation.Header,
        Type = SecuritySchemeType.Http,
        Scheme = "bearer",
        BearerFormat = "JWT",
        Reference = new OpenApiReference
        {
            Type = ReferenceType.SecurityScheme,
            Id = "Bearer"
        }
    };
    c.AddSecurityDefinition("Bearer", securityScheme);
    c.AddSecurityRequirement(new OpenApiSecurityRequirement
    {
        { securityScheme, new[] { "Bearer" } }
    });
});

builder.Services.AddDbContext<AppDbContext>(opt =>
    opt.UseSqlite(builder.Configuration.GetConnectionString("Default")));

builder.Services.AddScoped<IUserRepository, EfUserRepository>();
builder.Services.AddScoped<IProductRepository, EfProductRepository>();
builder.Services.AddScoped<IOrderRepository, EfOrderRepository>();
builder.Services.AddScoped<IPaymentRepository, EfPaymentRepository>();
builder.Services.AddScoped<ISubscriptionRepository, EfSubscriptionRepository>();

// 🟢 REGISTRO DO NOVO SERVIÇO DE AUTENTICAÇÃO
builder.Services.AddScoped<IAuthService, AuthService>();
builder.Services.AddScoped<IUserService, UserService>();

var app = builder.Build();

app.UseMiddleware<GlobalExceptionMiddleware>();

app.UseSwagger();
app.UseSwaggerUI(c => c.SwaggerEndpoint("/swagger/v1/swagger.json", "GearShop API v1"));

app.UseCors("CorsPolicyFrontend");

app.UseHttpsRedirection();

// 🟢 HABILITAR MIDDLEWARE DE AUTENTICAÇÃO E AUTORIZAÇÃO
app.UseAuthentication();
app.UseAuthorization();
// =====================================================================

app.MapControllers();

app.Run();