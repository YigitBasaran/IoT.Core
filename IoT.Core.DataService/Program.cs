using System.Net.Http.Headers;
using System.Text;
using Consul;
using FluentValidation;
using IoT.Core.CommonInfrastructure.Auth;
using IoT.Core.CommonInfrastructure.Database.Repo;
using IoT.Core.CommonInfrastructure.EventBus.Extensions;
using IoT.Core.CommonInfrastructure.EventBus.Publisher;
using IoT.Core.CommonInfrastructure.Exception;
using IoT.Core.CommonInfrastructure.Extensions;
using IoT.Core.DataService.Configuration;
using IoT.Core.DataService.Controllers.Mapping;
using IoT.Core.DataService.EventConsumer;
using IoT.Core.DataService.Model;
using IoT.Core.DataService.Repo;
using IoT.Core.DataService.Repo.DbContext;
using IoT.Core.DataService.Service;
using MassTransit;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using Polly;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddConsulRegistration(builder.Configuration);

builder.Services.AddPostgresDbContext<DataDbContext, Data, Guid, DataRepo>(
    builder.Configuration
);

builder.Services.AddScoped<IDataRepo, DataRepo>();
builder.Services.AddScoped<IDataService, DataService>();

builder.Services.AddAutoMapper(typeof(DataProfile));

builder.Services.AddJwtAuthentication(JwtSettings.Secret);

builder.Services.AddSwaggerDocumentation(
    title: "IoT Core Data Service API",
    version: "v1",
    description: "API for managing IoT Data transactions."
);

builder.Services.AddControllers();

builder.Services.AddMassTransit(config =>
{
    config.AddConsumer<IoTDataGeneratedEventConsumer>();

    config.UsingRabbitMq((context, cfg) =>
    {
        cfg.Host("localhost", "/", h =>
        {
            h.Username("guest");
            h.Password("guest");
        });

        cfg.ReceiveEndpoint("iot_data_queue", e =>
        {
            e.ConfigureConsumer<IoTDataGeneratedEventConsumer>(context);
        });
    });
});

builder.Services.AddTransient<IEventPublisher, EventPublisher>();

var retryPolicy = Polly.Policy
    .Handle<HttpRequestException>()
    .OrResult<HttpResponseMessage>(r => r.StatusCode == System.Net.HttpStatusCode.InternalServerError)
    .WaitAndRetryAsync(3, retryAttempt => TimeSpan.FromSeconds(Math.Pow(2, retryAttempt)),
        (result, timeSpan, retryCount, context) =>
        {
            Console.WriteLine($"Retry {retryCount} - Waiting {timeSpan.TotalSeconds}s before next attempt.");
        });

var timeoutPolicy = Polly.Policy.TimeoutAsync<HttpResponseMessage>(TimeSpan.FromSeconds(5));

builder.Services.AddHttpClient(nameof(DataService), client =>
{
    client.BaseAddress = new Uri("http://localhost:5099");
    client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
})
.AddPolicyHandler(retryPolicy)
.AddPolicyHandler(timeoutPolicy);

var app = builder.Build();

await app.UseConsulAsync("data-service", builder.Configuration);

using (var scope = app.Services.CreateScope())
{
    var dbContext = scope.ServiceProvider.GetRequiredService<DataDbContext>();
    dbContext.Database.Migrate();
}

app.UseSwaggerIfDev(app.Environment, "/swagger/v1/swagger.json", "IoT Data Service API v1");

app.UseGlobalMiddlewares();

app.MapControllers();

app.Run();
