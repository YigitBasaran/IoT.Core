using System.Net.Http.Headers;
using FluentValidation;
using IoT.Core.ClientService.Controllers.Mapping;
using IoT.Core.ClientService.Controllers.Validators;
using IoT.Core.ClientService.Model;
using IoT.Core.ClientService.Repo;
using IoT.Core.ClientService.Repo.DbContext;
using IoT.Core.ClientService.Service;
using IoT.Core.CommonInfrastructure.Auth;
using IoT.Core.CommonInfrastructure.EventBus.Extensions;
using IoT.Core.CommonInfrastructure.EventBus.Publisher;
using IoT.Core.CommonInfrastructure.Extensions;
using Microsoft.EntityFrameworkCore;
using Polly;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddConsulRegistration(builder.Configuration);

builder.Services.AddPostgresDbContext<ClientDbContext, Client, int, ClientRepo>(
    builder.Configuration
);

builder.Services.AddScoped<IClientRepo, ClientRepo>();
builder.Services.AddScoped<IClientService, ClientService>();

builder.Services.AddAutoMapper(typeof(ClientProfile));

builder.Services.AddValidatorsFromAssemblyContaining<AddClientRequestValidator>();

builder.Services.AddJwtAuthentication(JwtSettings.Secret);

builder.Services.AddSwaggerDocumentation(
    title: "IoT Core Client Service API",
    version: "v1",
    description: "API for managing Clients."
);

builder.Services.AddControllers();

builder.Services.AddMassTransitWithRabbitMq("localhost");

builder.Services.AddTransient<IEventPublisher, EventPublisher>();

// HttpClient with Polly Retry and Timeout Policies
var retryPolicy = Polly.Policy
    .Handle<HttpRequestException>()
    .OrResult<HttpResponseMessage>(r => r.StatusCode == System.Net.HttpStatusCode.InternalServerError)
    .WaitAndRetryAsync(3, retryAttempt => TimeSpan.FromSeconds(Math.Pow(2, retryAttempt)),
        (result, timeSpan, retryCount, context) =>
        {
            Console.WriteLine($"Retry {retryCount} - Waiting {timeSpan.TotalSeconds}s before next attempt.");
        });

var timeoutPolicy = Polly.Policy.TimeoutAsync<HttpResponseMessage>(TimeSpan.FromSeconds(5));

builder.Services.AddHttpClient(nameof(ClientService), client =>
    {
        client.BaseAddress = new Uri("http://localhost:5098");
        client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
    })
    .AddPolicyHandler(retryPolicy)
    .AddPolicyHandler(timeoutPolicy);

var app = builder.Build();

await app.UseConsulAsync("client-service", builder.Configuration);

using (var scope = app.Services.CreateScope())
{
    var dbContext = scope.ServiceProvider.GetRequiredService<ClientDbContext>();
    dbContext.Database.Migrate();
}

app.UseSwaggerIfDev(app.Environment, "/swagger/v1/swagger.json", "IoT Client Service API v1");

app.UseGlobalMiddlewares();

app.MapControllers();

app.Run();