using Consul;
using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace IoT.Core.CommonInfrastructure.Extensions;

public static class ConsulExtensions
{
    public static IServiceCollection AddConsulRegistration(this IServiceCollection services, IConfiguration configuration)
    {
        services.AddSingleton<IConsulClient, ConsulClient>(p => new ConsulClient(c =>
        {
            c.Address = new Uri(configuration["ConsulConfig:Host"] ?? "http://localhost:8500");
        }));

        return services;
    }

    public static async Task UseConsulAsync(this IApplicationBuilder app, string serviceName, IConfiguration configuration)
    {
        var consulClient = app.ApplicationServices.GetRequiredService<IConsulClient>();
        var serviceAddress = configuration["ASPNETCORE_URLS"]?.Split(';').FirstOrDefault() ?? "http://localhost:5000";
        var uri = new Uri(serviceAddress);
        var registration = new AgentServiceRegistration
        {
            ID = Guid.NewGuid().ToString(),
            Name = serviceName,
            Address = uri.Host,
            Port = uri.Port
        };

        await consulClient.Agent.ServiceRegister(registration);
    }
}