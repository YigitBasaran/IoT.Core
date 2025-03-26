using MassTransit;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using System.Threading.Tasks;

namespace IoT.Core.CommonInfrastructure.EventBus.Consumer
{
    public class EventConsumer<TEvent> : IConsumer<TEvent>
        where TEvent : class
    {
        private readonly ILogger<EventConsumer<TEvent>> _logger;
        private readonly IServiceScopeFactory _serviceScopeFactory;

        public EventConsumer(
            ILogger<EventConsumer<TEvent>> logger,
            IServiceScopeFactory serviceScopeFactory)
        {
            _logger = logger;
            _serviceScopeFactory = serviceScopeFactory;
        }

        public async Task Consume(ConsumeContext<TEvent> context)
        {
            var @event = context.Message;
            _logger.LogInformation($"Received event of type {typeof(TEvent).Name}");

            // Create a Scoped Dependency Injection Context
            using (var scope = _serviceScopeFactory.CreateScope())
            {
                var eventHandler = scope.ServiceProvider.GetRequiredService<IEventHandler<TEvent>>();
                await eventHandler.HandleAsync(@event, context.CancellationToken);
            }
        }
    }
}