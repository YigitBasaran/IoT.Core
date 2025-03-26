using MassTransit;
using Microsoft.Extensions.Logging;
using System.Threading;
using System.Threading.Tasks;

namespace IoT.Core.CommonInfrastructure.EventBus.Publisher
{
    public class EventPublisher : IEventPublisher
    {
        private readonly ILogger<EventPublisher> _logger;
        private readonly IBus _bus;

        public EventPublisher(ILogger<EventPublisher> logger, IBus bus)
        {
            _logger = logger;
            _bus = bus;
        }

        public async Task PublishAsync<TEvent>(string exchange, string routingKey, TEvent @event, CancellationToken cancellationToken = default)
            where TEvent : class
        {
            await _bus.Publish(@event, cancellationToken);
            _logger.LogInformation($"Published event of type {typeof(TEvent).Name} to Exchange: {exchange}, Routing Key: {routingKey}");
        }
    }
}