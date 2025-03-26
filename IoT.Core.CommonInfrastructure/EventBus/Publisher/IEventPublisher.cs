namespace IoT.Core.CommonInfrastructure.EventBus.Publisher
{
    public interface IEventPublisher
    {
        Task PublishAsync<TEvent>(string exchange, string routingKey, TEvent @event, CancellationToken cancellationToken = default)
            where TEvent : class;
    }
}