using Microsoft.OpenApi.Models;
using Ocelot.DependencyInjection;
using Ocelot.Middleware;
using Ocelot.Provider.Consul;
using Consul;

var builder = WebApplication.CreateBuilder(args);

// Load Ocelot configuration
builder.Configuration.AddJsonFile("ocelot.json", optional: false, reloadOnChange: true);

// Register Ocelot & Consul services
builder.Services.AddOcelot().AddConsul();
builder.Services.AddSingleton<IConsulClient, ConsulClient>(p => new ConsulClient(config =>
{
    var address = builder.Configuration["ConsulConfig:Host"];
    config.Address = new Uri(address ?? "http://localhost:8500");
}));

// Register Swagger for API Gateway
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(c =>
{
    c.SwaggerDoc("v1", new OpenApiInfo
    {
        Title = "IoT API Gateway",
        Version = "v1",
        Description = "API Gateway for IoT Microservices"
    });
});
builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowFrontend",
        policy =>
        {
            policy.WithOrigins("http://localhost:53550")
                .AllowAnyHeader()
                .AllowAnyMethod();
        });
});


var app = builder.Build();

// Enable Swagger UI
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI(c =>
    {
        c.SwaggerEndpoint("/device/swagger/v1/swagger.json", "Device Service API");
        c.SwaggerEndpoint("/client/swagger/v1/swagger.json", "Client Service API");
        c.SwaggerEndpoint("/data/swagger/v1/swagger.json", "Data Service API");
        c.SwaggerEndpoint("/auth/swagger/v1/swagger.json", "Auth Service API");
    });

}
app.UseCors("AllowFrontend");

// Use Ocelot Middleware
await app.UseOcelot();

app.Run();