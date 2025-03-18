using Microsoft.OpenApi.Models;
using Ocelot.DependencyInjection;
using Ocelot.Middleware;

var builder = WebApplication.CreateBuilder(args);

// Ocelot konfigürasyon dosyasýný ekle
builder.Configuration.AddJsonFile("ocelot.json");

// Ocelot servislerini ekle
builder.Services.AddOcelot();

// Swagger servisini ekle
builder.Services.AddSwaggerGen(c =>
{
    c.SwaggerDoc("v1", new OpenApiInfo { Title = "API Gateway", Version = "v1" });
});

var app = builder.Build();

// Swagger middleware'ini ekle
app.UseSwagger();
app.UseSwaggerUI(c =>
{
    c.SwaggerEndpoint("/swagger/v1/swagger.json", "API Gateway v1");

    // Mikroservislerin Swagger dokümantasyonunu ekle
    c.SwaggerEndpoint("/clients/swagger/v1/swagger.json", "Client Service API");
    c.SwaggerEndpoint("/devices/swagger/v1/swagger.json", "Device Service API");
    c.SwaggerEndpoint("/transactions/swagger/v1/swagger.json", "Data Transaction Service API");
});

// Ocelot middleware'ini ekle
app.UseOcelot().Wait();

app.Run();