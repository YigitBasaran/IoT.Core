using Consul;
using IoT.Core.AuthService.EventConsumer.ConsumedEvent;
using IoT.Core.AuthService.Service;
using IoT.Core.CommonInfrastructure.EventBus.Shared;
using MassTransit;

namespace IoT.Core.AuthService.EventConsumer
{
    public class ClientAddedEventConsumer : IConsumer<ClientAddedEvent>
    {
        private readonly ILogger<ClientAddedEventConsumer> _logger;
        private readonly IAuthService _authService;

        public ClientAddedEventConsumer(
            ILogger<ClientAddedEventConsumer> logger,
            IAuthService authService)
        {
            _logger = logger;
            _authService = authService;
        }

        public async Task Consume(ConsumeContext<ClientAddedEvent> context)
        {
            try
            {
                var @event = context.Message;
                _logger.LogInformation($"Received ClientAddedEvent with client id {@event.ClientId}.");

                await _authService.CreateUserAsync(@event.ClientId, @event.Name, @event.Role);
            }
            catch (Exception e)
            {
                _logger.LogInformation($"Failed to add user with Id {context.Message.ClientId}. With message: {e.Message}");
            }
        }
    }
}