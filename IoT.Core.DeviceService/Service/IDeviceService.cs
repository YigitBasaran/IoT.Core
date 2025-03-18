using IoT.Core.DeviceService.Controllers.Dto;

namespace IoT.Core.DeviceService.Service;

public interface IDeviceService
{
    Task<List<Model.Device>> GetDevicesAsync();
    Task<Model.Device> GetDeviceByIdAsync(Guid id);
    Task<Model.Device> CreateDeviceAsync(AddDeviceRequestDto request);
    Task UpdateDeviceAsync(UpdateDeviceRequestDto request);
    Task DeleteDeviceAsync(Guid id);
    Task<List<Model.Device>> GetDevicesByCustomerId(int customerId);
}