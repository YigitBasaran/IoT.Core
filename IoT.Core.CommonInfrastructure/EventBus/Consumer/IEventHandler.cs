namespace IoT.Core.CommonInfrastructure.EventBus.Consumer
{
    public interface IEventHandler<TEvent>
        where TEvent : class
    {
        Task HandleAsync(TEvent @event, CancellationToken cancellationToken = default);
    }
}