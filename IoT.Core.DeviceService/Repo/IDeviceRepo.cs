namespace IoT.Core.DeviceService.Repo;
using IoT.Core.DeviceService.Model;
public interface IDeviceRepo
{
    Task<List<Device>> GetDevicesAsync();
    Task<Device> GetDeviceByIdAsync(Guid id);
    Task CreateDeviceAsync(Device device);
    Task UpdateDeviceAsync(Device device);
    Task DeleteDeviceAsync(Guid id);
    Task<List<Device>> GetDevicesByCustomerId(int customerId);
}

