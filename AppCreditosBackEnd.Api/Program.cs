using AppCreditosBackEnd.Application.Interfaces;
using AppCreditosBackEnd.Application.Services;
using AppCreditosBackEnd.Domain.Interfaces;
using AppCreditosBackEnd.Infrastructure.DbContext;
using AppCreditosBackEnd.Infrastructure.Repositories;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
builder.Services.AddDbContext<CreditPlatformDbContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));

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
builder.Services.AddScoped<IAuthService, AuthService>();
builder.Services.AddScoped<ICreditApplicationService, CreditApplicationService>();
builder.Services.AddScoped<ICreditEvaluationService, CreditEvaluationService>();
builder.Services.AddScoped<IUserService, UserService>();
var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();
// Database migration
using (var scope = app.Services.CreateScope())
{
    var context = scope.ServiceProvider.GetRequiredService<CreditPlatformDbContext>();
    context.Database.EnsureCreated();
}

app.Run();
