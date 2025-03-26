using Consul;
using IoT.Core.AuthService.EventConsumer.ConsumedEvent;
using IoT.Core.AuthService.Service;
using IoT.Core.CommonInfrastructure.EventBus.Shared;
using MassTransit;

namespace IoT.Core.AuthService.EventConsumer
{
    public class ClientDeletedEventConsumer : IConsumer<ClientDeletedEvent>
    {
        private readonly ILogger<ClientDeletedEventConsumer> _logger;
        private readonly IAuthService _authService;

        public ClientDeletedEventConsumer(
            ILogger<ClientDeletedEventConsumer> logger,
            IAuthService authService)
        {
            _logger = logger;
            _authService = authService;
        }

        public async Task Consume(ConsumeContext<ClientDeletedEvent> context)
        {
            try
            {
                var @event = context.Message;
                _logger.LogInformation($"Received ClientDeletedEvent with client id {@event.ClientId}.");

                await _authService.DeleteUserAsync(@event.ClientId);
            }
            catch (Exception e)
            {
                _logger.LogInformation($"Failed to delete user with Id {context.Message.ClientId}. With message: {e.Message}");
            }
        }
    }
}