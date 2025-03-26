using Consul;

namespace IoT.Core.Gateway;

public class ConsulServiceRegistration : IHostedService
{
    private readonly IConsulClient _consulClient;
    private readonly IConfiguration _configuration;
    private readonly ILogger<ConsulServiceRegistration> _logger;
    private string _registrationId;

    public ConsulServiceRegistration(IConsulClient consulClient, IConfiguration configuration, ILogger<ConsulServiceRegistration> logger)
    {
        _consulClient = consulClient;
        _configuration = configuration;
        _logger = logger;
        _registrationId = Guid.NewGuid().ToString();
    }

    public async Task StartAsync(CancellationToken cancellationToken)
    {
        var serviceName = _configuration["ServiceConfig:ServiceName"];
        var serviceAddress = _configuration["ServiceConfig:ServiceAddress"];
        var servicePort = int.Parse(_configuration["ServiceConfig:ServicePort"] ?? "5000");

        var registration = new AgentServiceRegistration
        {
            ID = _registrationId,
            Name = serviceName,
            Address = serviceAddress,
            Port = servicePort,
            Check = new AgentServiceCheck
            {
                HTTP = $"{serviceAddress}:{servicePort}/health",
                Interval = TimeSpan.FromSeconds(10),
                Timeout = TimeSpan.FromSeconds(5),
                DeregisterCriticalServiceAfter = TimeSpan.FromMinutes(1)
            }
        };

        _logger.LogInformation("Registering API Gateway with Consul...");
        await _consulClient.Agent.ServiceRegister(registration, cancellationToken);
        _logger.LogInformation("API Gateway registered successfully.");
    }

    public async Task StopAsync(CancellationToken cancellationToken)
    {
        _logger.LogInformation("Deregistering API Gateway from Consul...");
        await _consulClient.Agent.ServiceDeregister(_registrationId, cancellationToken);
    }
}