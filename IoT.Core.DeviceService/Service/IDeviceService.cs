using IoT.Core.DeviceService.Controllers.Dto;

namespace IoT.Core.DeviceService.Service;

public interface IDeviceService
{
    Task<List<Model.Device>> GetDevicesAsync();
    Task<Model.Device> GetDeviceByIdAsync(string devEui);
    Task<Model.Device> CreateDeviceAsync(AddDeviceRequestDto request);
    Task UpdateDeviceNameAsync(UpdateDeviceNameRequestDto request);
    Task UpdateDeviceLocationAsync(UpdateDeviceLocationRequestDto request);
    Task DeleteDeviceAsync(string devEui, string jwt);
    Task<List<Model.Device>> GetDevicesByClientIdAsync(int clientId);
    Task DeleteDevicesByClientId(int clientId, string jwt);
}