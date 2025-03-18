using IoT.Core.DeviceService.Configuration;
using IoT.Core.DeviceService.Repo;
using IoT.Core.DeviceService.Service;
using MongoDB.Bson.Serialization.Serializers;
using MongoDB.Bson.Serialization;
using MongoDB.Bson;
using MongoDB.Driver;
using IoT.Core.DeviceService.Middlewares;
using IoT.Core.DeviceService.Controllers.Mapping;
using FluentValidation;
using IoT.Core.DeviceService.Controllers.Validators;
using Microsoft.OpenApi.Models;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddControllers();
builder.Services.AddValidatorsFromAssemblyContaining<AddDeviceRequestValidator>();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(c =>
{
    c.EnableAnnotations(); // Enables annotations from Swashbuckle
    c.SwaggerDoc("v1", new OpenApiInfo
    {
        Title = "IoT Core Device Service API",
        Version = "v1",
        Description = "API for managing IoT devices."
    });
});
// Bind MongoDB settings from appsettings.json
var dbSettings = builder.Configuration.GetSection("DbSettings").Get<DbSettings>();

builder.Services.AddSingleton(dbSettings);

// Register MongoClient as scoped
builder.Services.AddScoped<IMongoClient>(serviceProvider => new MongoClient(dbSettings.ConnectionString));
// Ensure MongoDB uses GuidRepresentation.Standard 
BsonSerializer.RegisterSerializer(new GuidSerializer(GuidRepresentation.Standard));
builder.Services.AddScoped<IDeviceRepo, DeviceRepo>();
builder.Services.AddScoped<IDeviceService, DeviceService>();
builder.Services.AddAutoMapper(typeof(DeviceProfile)); // Register AutoMapper


var app = builder.Build();

app.UseMiddleware<GlobalExceptionHandlerMiddleware>();


// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI(c => { c.SwaggerEndpoint("/swagger/v1/swagger.json", "IoT Device Service API v1"); });
}

app.UseAuthorization();

app.MapControllers();

app.Run();