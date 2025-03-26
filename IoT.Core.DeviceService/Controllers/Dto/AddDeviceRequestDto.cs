using IoT.Core.DeviceService.Model;

namespace IoT.Core.DeviceService.Controllers.Dto;

public record AddDeviceRequestDto(string DevEui, string Name, int ClientId, Location Location);