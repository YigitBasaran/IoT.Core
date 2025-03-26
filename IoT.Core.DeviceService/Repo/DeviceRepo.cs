using IoT.Core.DeviceService.Configuration;
using IoT.Core.DeviceService.Model;
using MongoDB.Driver;
using IoT.Core.CommonInfrastructure.Database.Repo.Implementations;
using IoT.Core.CommonInfrastructure.Extensions.DbSettings;
using IoT.Core.DeviceService.Configuration;
namespace IoT.Core.DeviceService.Repo;

public class DeviceRepo : BaseMongoRepository<Device, string>, IDeviceRepo

{
    public DeviceRepo(IMongoClient mongoClient, DbSettings dbSettings)
        : base(mongoClient, dbSettings.DatabaseName, dbSettings.CollectionName)
    {
    }

    public async Task<List<Device>> FindDevicesByClientIdAsync(int clientId)
    {
        return (await FindAsync(device => device.ClientId == clientId)).ToList();
    }

    public async Task<bool> IsThereAnyEntityWithSameDevEuiAsync(string devEui)
    {
        return (await FindAsync(device => (device.Id == devEui))).ToList().Count != 0;
    }

    public async Task<bool> IsThereAnyEntityWithSameNameForSameClientAsync(int clientId, string devEui, string name)
    {
        return (await FindAsync(device =>
                device.ClientId == clientId && 
                device.Name == name &&        
                device.Id != devEui           
        )).ToList().Count != 0;
    }
}