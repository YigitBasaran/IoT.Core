using FluentValidation;
using IoT.Core.AuthService.Controllers.Mapping;
using IoT.Core.AuthService.Controllers.Validators;
using IoT.Core.AuthService.EventConsumer;
using IoT.Core.AuthService.Model;
using IoT.Core.AuthService.Repo;
using IoT.Core.AuthService.Repo.DbContext;
using IoT.Core.AuthService.Service;
using IoT.Core.CommonInfrastructure.Auth;
using IoT.Core.CommonInfrastructure.Extensions;
using MassTransit;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

// FluentValidation
builder.Services.AddValidatorsFromAssemblyContaining<LoginRequestValidator>();

// Consul
builder.Services.AddConsulRegistration(builder.Configuration);

// PostgreSQL + Repo
builder.Services.AddPostgresDbContext<UserDbContext, User, int, UserRepo>(
    builder.Configuration
);

// Auth Service
builder.Services.AddScoped<IUserRepo, UserRepo>();
builder.Services.AddScoped<IAuthService, AuthService>();

// AutoMapper
builder.Services.AddAutoMapper(typeof(UserProfile));

// Swagger
builder.Services.AddControllers();
builder.Services.AddSwaggerDocumentation(
    title: "IoT Core Auth Service API",
    version: "v1",
    description: "API for managing Authorization & Authentication."
);

// JWT Authentication
builder.Services.AddJwtAuthentication(JwtSettings.Secret);

// MassTransit + Consumers
builder.Services.AddMassTransit(config =>
{
    config.AddConsumer<ClientDeletedEventConsumer>();
    config.AddConsumer<ClientAddedEventConsumer>();

    config.UsingRabbitMq((context, cfg) =>
    {
        cfg.Host("localhost", "/", h =>
        {
            h.Username("guest");
            h.Password("guest");
        });

        cfg.ReceiveEndpoint("auth_service_client_deleted_queue", e =>
        {
            e.ConfigureConsumer<ClientDeletedEventConsumer>(context);
        });

        cfg.ReceiveEndpoint("auth_service_client_added_queue", e =>
        {
            e.ConfigureConsumer<ClientAddedEventConsumer>(context);
        });
    });
});

var app = builder.Build();

// Consul Service Register
await app.UseConsulAsync("auth-service", builder.Configuration);

// DB Migration
using (var scope = app.Services.CreateScope())
{
    var dbContext = scope.ServiceProvider.GetRequiredService<UserDbContext>();
    dbContext.Database.Migrate();
}

// Swagger (only dev)
app.UseSwaggerIfDev(app.Environment, "/swagger/v1/swagger.json", "IoT Auth Service API v1");

// Global Middlewares (Exception, Auth, Authorization)
app.UseGlobalMiddlewares();

// Routing
app.MapControllers();

app.Run();
