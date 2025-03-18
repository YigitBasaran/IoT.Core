using IoT.Core.DeviceService.Model;

namespace IoT.Core.DeviceService.Controllers.Dto
{
    public record UpdateDeviceRequestDto(Guid Id, string DevEUI, string Name, int CustomerId, Location Location);
}
