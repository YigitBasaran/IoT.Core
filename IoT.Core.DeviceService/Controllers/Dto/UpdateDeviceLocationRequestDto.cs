using IoT.Core.DeviceService.Model;

namespace IoT.Core.DeviceService.Controllers.Dto
{
    public record UpdateDeviceLocationRequestDto(string DevEui, Location Location);

}
