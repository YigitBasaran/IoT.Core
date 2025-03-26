using IoT.Core.DeviceService.Model;

namespace IoT.Core.DeviceService.Controllers.Dto
{
    public record UpdateDeviceNameRequestDto(string DevEui, string Name);
}
