namespace IoT.Core.CommonInfrastructure.EventBus.Consumer
{
    public interface IEventConsumer
    {
        Task StartConsumingAsync(CancellationToken cancellationToken = default);
    }
}