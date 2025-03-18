
using IoT.Core.DeviceService.Configuration;
using IoT.Core.DeviceService.Model;
using IoT.Core.DeviceService.Model.Exceptions;
using Microsoft.Extensions.Options;
using MongoDB.Driver;

namespace IoT.Core.DeviceService.Repo;
public class DeviceRepo : IDeviceRepo
{
    private readonly IMongoCollection<Model.Device> _devices;

    public DeviceRepo(IMongoClient mongoClient, DbSettings dbSettings)
    {
        var database = mongoClient.GetDatabase(dbSettings.DatabaseName);
        _devices = database.GetCollection<Model.Device>(dbSettings.CollectionName);
    }
    public async Task CreateDeviceAsync(Model.Device device)
    {
        await CheckDevEuiDuplication(device);
        await CheckDeviceNameDuplication(device);
        await _devices.InsertOneAsync(device);
    }

    public async Task DeleteDeviceAsync(Guid id)
    {
        await _devices.DeleteOneAsync(device => device.Id == id);
    }

    public async Task<List<Model.Device>> GetDevicesByCustomerId(int customerId)
    {
        return await _devices.Find(device => device.CustomerId == customerId).ToListAsync();
    }

    public async Task<Model.Device> GetDeviceByIdAsync(Guid id)
    {
        return await _devices.Find(device => device.Id == id).FirstOrDefaultAsync();

    }

    public async Task<List<Model.Device>> GetDevicesAsync()
    {
        return await _devices.Find(device => true).ToListAsync();
    }

    public async Task UpdateDeviceAsync(Model.Device device)
    {
        await CheckDevEuiDuplication(device);
        await CheckDeviceNameDuplication(device);
        await _devices.ReplaceOneAsync(d => d.Id == device.Id, device);
    }

    private async Task CheckDeviceNameDuplication(Model.Device device)
    {
        var filter = Builders<Model.Device>.Filter.Eq(d => d.Name, device.Name);
        var count = await _devices.CountDocumentsAsync(filter);

        if (count > 0)
        {
            throw new DeviceNameDuplicationException(device.Name);
        }
    }

    private async Task CheckDevEuiDuplication(Model.Device device)
    {
        var filter = Builders<Model.Device>.Filter.Eq(d => d.DevEUI, device.DevEUI);
        var count = await _devices.CountDocumentsAsync(filter);

        if (count > 0)
        {
            throw new DevEuiDuplicationException(device.DevEUI);
        }
    }
}

