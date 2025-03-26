using IoT.Core.AuthService.Model;

namespace IoT.Core.AuthService.EventConsumer.ConsumedEvent
{
    public record ClientAddedEvent(int ClientId, string Name, RoleEnum Role);

}
