using AutoMapper;
using IoT.Core.DeviceService.Controllers.Dto;
using IoT.Core.DeviceService.Model.Exceptions;
using IoT.Core.DeviceService.Repo;

namespace IoT.Core.DeviceService.Service;

public class DeviceService : IDeviceService
{
    private readonly IDeviceRepo deviceRepo;
    private readonly IMapper mapper;

    public DeviceService(IDeviceRepo deviceRepo, IMapper mapper)
    {
        this.deviceRepo = deviceRepo;
        this.mapper = mapper;
    }

    public async Task<List<Model.Device>> GetDevicesAsync()
    {
        return await deviceRepo.GetDevicesAsync();
    }

    public async Task<Model.Device> GetDeviceByIdAsync(Guid id)
    {
        try
        {
            var device = await deviceRepo.GetDeviceByIdAsync(id);

            if (device == null)
            {
                throw new DeviceNotFoundException(id);
            }

            return device;
        }
        catch (Exception e)
        {
            Console.WriteLine(e);
            throw;
        }
    }

    public async Task<Model.Device> CreateDeviceAsync(AddDeviceRequestDto request)
    {
        var device = Model.Device.OnCreate(request.DevEUI, request.Name, request.CustomerId, request.Location);
        await deviceRepo.CreateDeviceAsync(device);
        return device;
    }

    public async Task UpdateDeviceAsync(UpdateDeviceRequestDto request)
    {
        var existingDevice = await deviceRepo.GetDeviceByIdAsync(request.Id);
        if (existingDevice == null)
        {
            throw new DeviceNotFoundException(request.Id);
        }

        var updatedDevice = mapper.Map<Model.Device>(request);
        if (existingDevice.isDirty(updatedDevice))
        {
            await deviceRepo.UpdateDeviceAsync(updatedDevice);

        }
    }

    public async Task DeleteDeviceAsync(Guid id)
    {
        var device = await deviceRepo.GetDeviceByIdAsync(id);
        if (device == null)
        {
            throw new DeviceNotFoundException(id);
        }

        await deviceRepo.DeleteDeviceAsync(id);
    }

    public async Task<List<Model.Device>> GetDevicesByCustomerId(int customerId)
    {
        var devices = await deviceRepo.GetDevicesByCustomerId(customerId);
        return devices;
    }
}