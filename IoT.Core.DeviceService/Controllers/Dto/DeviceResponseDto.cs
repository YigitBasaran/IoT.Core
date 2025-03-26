using IoT.Core.DeviceService.Model;

namespace IoT.Core.DeviceService.Controllers.Dto
{
    public record DeviceResponseDto(string DevEui, string Name, int ClientId, Location Location);
}