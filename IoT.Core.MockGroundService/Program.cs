using IoT.Core.CommonInfrastructure.EventBus.Consumer;
using IoT.Core.CommonInfrastructure.EventBus.Extensions;
using IoT.Core.CommonInfrastructure.EventBus.Publisher;
using IoT.Core.MockGroundService.Controllers;
using Microsoft.OpenApi.Models;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(c =>
{
    c.EnableAnnotations(); // Enables annotations from Swashbuckle
    c.SwaggerDoc("v1", new OpenApiInfo
    {
        Title = "IoT Core Mock Ground Service API",
        Version = "v1",
        Description = "API for managing Mock Ground."
    });
}); 

builder.Services.AddSingleton<IoTDataGenerator>();

// Register MassTransit
builder.Services.AddMassTransitWithRabbitMq("localhost");

// Register EventPublisher
builder.Services.AddTransient<IEventPublisher, EventPublisher>();


builder.Services.AddLogging(logging => logging.AddConsole());

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI(c => { c.SwaggerEndpoint("/swagger/v1/swagger.json", "IoT Mock Ground Service API v1"); });
}

app.UseAuthorization();

app.MapControllers();

app.Run();
