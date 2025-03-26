using IoT.Core.CommonInfrastructure.Database.Repo;

namespace IoT.Core.DeviceService.Repo;
using IoT.Core.DeviceService.Model;

public interface IDeviceRepo : IRepository<Device, string>
{
    public Task<List<Device>> FindDevicesByClientIdAsync(int clientId);

    public Task<bool> IsThereAnyEntityWithSameDevEuiAsync(string devEui);

    public Task<bool> IsThereAnyEntityWithSameNameForSameClientAsync(int clientId, string devEui, string name);
}