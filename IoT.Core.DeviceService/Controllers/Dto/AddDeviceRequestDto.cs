using IoT.Core.DeviceService.Model;

namespace IoT.Core.DeviceService.Controllers.Dto;

public record AddDeviceRequestDto(string DevEUI, string Name, int CustomerId, Location Location);