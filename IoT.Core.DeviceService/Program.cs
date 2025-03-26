using FluentValidation;
using IoT.Core.CommonInfrastructure.Auth;
using IoT.Core.CommonInfrastructure.Extensions;
using IoT.Core.CommonInfrastructure.Extensions.DbSettings;
using IoT.Core.DeviceService.Configuration;
using IoT.Core.DeviceService.Controllers.Mapping;
using IoT.Core.DeviceService.Controllers.Validators;
using IoT.Core.DeviceService.Model;
using IoT.Core.DeviceService.Repo;
using IoT.Core.DeviceService.Service;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllers();
builder.Services.AddValidatorsFromAssemblyContaining<AddDeviceRequestValidator>();

builder.Services.AddConsulRegistration(builder.Configuration);

builder.Services.AddJwtAuthentication(JwtSettings.Secret);

builder.Services.AddSwaggerDocumentation(
    title: "IoT Core Device Service API",
    version: "v1",
    description: "API for managing IoT devices."
);

builder.Services.AddMongoDatabase<Device, string, DeviceRepo>(
    builder.Configuration
);
builder.Services.AddSingleton<DbSettings>(sp => DbSettings.MapFromMongoDbSettingsToDbSettings(sp.GetRequiredService<MongoDbSettings>()));

builder.Services.AddScoped<IDeviceRepo, DeviceRepo>();
builder.Services.AddScoped<IDeviceService, DeviceService>();

builder.Services.AddAutoMapper(typeof(DeviceProfile));

builder.Services.AddSingleton<DatabaseInitializer>();

var app = builder.Build();

await app.UseConsulAsync("device-service", builder.Configuration);

using (var scope = app.Services.CreateScope())
{
    var initializer = scope.ServiceProvider.GetRequiredService<DatabaseInitializer>();
    await initializer.SeedAsync();
}

app.UseSwaggerIfDev(app.Environment, "/swagger/v1/swagger.json", "IoT Device Service API v1");

app.UseGlobalMiddlewares();

app.MapControllers();

app.Run();