using System.Text;
using AppCreditosBackEnd.Application.Interfaces;
using AppCreditosBackEnd.Application.Services;
using AppCreditosBackEnd.Domain.Interfaces;
using AppCreditosBackEnd.Infrastructure.DbContext;
using AppCreditosBackEnd.Infrastructure.Repositories;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddControllers();

// Add health checks
builder.Services.AddHealthChecks()
    .AddDbContext<CreditPlatformDbContext>();

// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();

// Configuración de CORS
builder.Services.AddCors();
builder.Services.AddSwaggerGen(c =>
{
    c.SwaggerDoc("v1", new OpenApiInfo { Title = "Credit Platform API", Version = "v1" });

    // JWT Configuration for Swagger
    c.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
    {
        Description = "JWT Authorization header using the Bearer scheme",
        Name = "Authorization",
        In = ParameterLocation.Header,
        Type = SecuritySchemeType.ApiKey,
        Scheme = "Bearer"
    });

    c.AddSecurityRequirement(new OpenApiSecurityRequirement
    {
        {
            new OpenApiSecurityScheme
            {
                Reference = new OpenApiReference
                {
                    Type = ReferenceType.SecurityScheme,
                    Id = "Bearer"

                }
            },
            Array.Empty<string>()
        }
    });
});
builder.Services.AddDbContext<CreditPlatformDbContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));

// JWT Configuration
var jwtSettings = builder.Configuration.GetSection("JwtSettings");
builder.Services.Configure<AppCreditosBackEnd.Application.Helpers.JwtSettings>(jwtSettings);

var key = Encoding.ASCII.GetBytes(jwtSettings["Secret"]!);
builder.Services.AddAuthentication(options =>
{
    options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
    options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
})
.AddJwtBearer(options =>
{
    options.TokenValidationParameters = new TokenValidationParameters
    {
        ValidateIssuerSigningKey = true,
        IssuerSigningKey = new SymmetricSecurityKey(key),
        ValidateIssuer = true,
        ValidIssuer = jwtSettings["Issuer"],
        ValidateAudience = true,
        ValidAudience = jwtSettings["Audience"],
        ValidateLifetime = true,
        ClockSkew = TimeSpan.Zero
    };
    
    // Evento para manejar tokens inválidos (logout)
    options.Events = new JwtBearerEvents
    {
        OnTokenValidated = context =>
        {
            // Aquí podríamos verificar si el token está en la lista negra
            // pero para mantener la simplicidad, no implementamos esto aquí
            return Task.CompletedTask;
        }
    };
});
// Registrar manualmente los mapeos de AutoMapper
builder.Services.AddSingleton(provider => {
    var config = new AutoMapper.MapperConfiguration(cfg => {
        cfg.AddProfile<AppCreditosBackEnd.Application.Mappings.MappingProfile>();
    });
    return config.CreateMapper();
});
builder.Services.AddScoped<ICreditApplicationRepository, CreditApplicationRepository>();
// Nota: Debido a una discrepancia en los nombres de carpetas (AppCredtiosBackEnd vs AppCreditosBackEnd)
// tenemos que registrar el repositorio de esta manera
builder.Services.AddScoped<IUserRepository>(sp => {
    var context = sp.GetRequiredService<CreditPlatformDbContext>();
    return new AppCreditosBackEnd.Infrastructure.Repositories.UserRepository(context);
});
builder.Services.AddScoped<IAuditLogRepository, AuditLogRepository>();

// Nuevos repositorios bancarios
builder.Services.AddScoped<IAccountRepository, AccountRepository>();
builder.Services.AddScoped<IEmployerRepository, EmployerRepository>();
builder.Services.AddScoped<ICardApplicationRepository, CardApplicationRepository>();
builder.Services.AddScoped<ICardRepository, CardRepository>();

// Servicios existentes
builder.Services.AddScoped<IAuditLogService, AuditLogService>();
builder.Services.AddScoped<IAuthService, AuthService>();
builder.Services.AddScoped<ICreditApplicationService, CreditApplicationService>();
builder.Services.AddScoped<ICreditEvaluationService, CreditEvaluationService>();
builder.Services.AddScoped<IUserService, UserService>();

// Nuevos servicios bancarios
builder.Services.AddScoped<IAccountService, AccountService>();
builder.Services.AddScoped<IEmployerService, EmployerService>();
builder.Services.AddScoped<IBankingCardApplicationService, CardApplicationService>();
builder.Services.AddScoped<ICardService, CardService>();
var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

// Añadimos CORS para permitir peticiones desde cualquier origen en desarrollo
app.UseCors(x => x
    .AllowAnyOrigin()
    .AllowAnyMethod()
    .AllowAnyHeader());

app.UseHttpsRedirection();

// Estos middleware son necesarios para la autenticación JWT
app.UseAuthentication();
app.UseAuthorization();

app.UseHttpsRedirection();

app.UseAuthorization();

// Health check endpoint
app.MapHealthChecks("/health");

app.MapControllers();
// Database migration
using (var scope = app.Services.CreateScope())
{
    var context = scope.ServiceProvider.GetRequiredService<CreditPlatformDbContext>();
    context.Database.EnsureCreated();
}

app.Run();
