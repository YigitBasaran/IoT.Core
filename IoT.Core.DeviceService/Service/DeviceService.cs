using System.Diagnostics;
using IoT.Core.DeviceService.Controllers.Dto;
using IoT.Core.DeviceService.Model.Exceptions;
using IoT.Core.DeviceService.Repo;
using System.Net.Http.Headers;
using IoT.Core.DeviceService.Model;

namespace IoT.Core.DeviceService.Service;

public class DeviceService : IDeviceService
{
    private readonly IDeviceRepo _deviceRepo;
    private readonly ILogger<DeviceService> _logger;

    public DeviceService(IDeviceRepo deviceRepo, ILogger<DeviceService> logger)
    {
        this._deviceRepo = deviceRepo;
        _logger = logger;
    }

    public async Task<List<Model.Device>> GetDevicesAsync()
    {
        return (await _deviceRepo.GetAllAsync()).ToList();
    }

    public async Task<Model.Device> GetDeviceByIdAsync(string devEui)
    {
        try
        {
            var device = await _deviceRepo.FindByIdAsync(devEui);

            if (device == null)
            {
                throw new DeviceNotFoundException(devEui);
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
        var device = Model.Device.OnCreate(request.DevEui, request.Name, request.ClientId, request.Location);

        await ValidateDevEui(request.DevEui, request.Name);
        await ValidateName(request.ClientId, request.DevEui, request.Name);

        await _deviceRepo.AddAsync(device);
        return device;
    }

    public async Task UpdateDeviceNameAsync(UpdateDeviceNameRequestDto request)
    {
        var existingDevice = await _deviceRepo.FindByIdAsync(request.DevEui);
        if (existingDevice == null)
        {
            throw new DeviceNotFoundException(request.DevEui);
        }

        await ValidateName(existingDevice.ClientId, request.DevEui, request.Name);

        if (!existingDevice.Name.Equals(request.Name))
        {
            existingDevice.OnUpdateName(request.Name);
            await _deviceRepo.UpdateAsync(existingDevice);
        }
    }

    public async Task UpdateDeviceLocationAsync(UpdateDeviceLocationRequestDto request)
    {
        var existingDevice = await _deviceRepo.FindByIdAsync(request.DevEui);
        if (existingDevice == null)
        {
            throw new DeviceNotFoundException(request.DevEui);
        }

        if (!existingDevice.Location.isEqual(request.Location))
        {
            existingDevice.OnUpdateLocation(request.Location);
            await _deviceRepo.UpdateAsync(existingDevice);
        }
    }

    public async Task DeleteDeviceAsync(string devEui, string jwt)
    {
        var device = await FindDeviceByDevEuiAsync(devEui);

        bool dataDeletedSuccessfully = await DeleteDataByDevEuiAsync(devEui, jwt);

        if (!dataDeletedSuccessfully)
        {
            _logger.LogError($"Failed to delete IoT data for device {devEui}.");
            throw new Exception("IoT data deletion failed. Device deletion aborted.");
        }

        await _deviceRepo.RemoveAsync(device);
    }

    private async Task<Device> FindDeviceByDevEuiAsync(string devEui)
    {
        var device = await _deviceRepo.FindByIdAsync(devEui);
        if (device == null)
        {
            throw new DeviceNotFoundException(devEui);
        }
        return device;
    }

    private async Task<bool> DeleteDataByDevEuiAsync(string devEui, string jwt)
    {
        try
        {
            using var httpClient = new HttpClient();
            httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", jwt);

            var response = await httpClient.DeleteAsync($"http://localhost:5099/api/data/device-id/{devEui}");

            if (!response.IsSuccessStatusCode)
            {
                _logger.LogError($"Failed to delete IoT data for device {devEui}. Status Code: {response.StatusCode}");
                return false;
            }

            return true;
        }
        catch (Exception ex)
        {
            _logger.LogError($"Error calling DataService to delete IoT data: {ex.Message}");
            return false;
        }
    }

    public async Task<List<Model.Device>> GetDevicesByClientIdAsync(int clientId)
    {
        var devices = await _deviceRepo.FindDevicesByClientIdAsync(clientId);
        return devices;
    }

    public async Task DeleteDevicesByClientId(int clientId, string jwt)
    {
        var devices = (await _deviceRepo.FindAsync(device => device.ClientId == clientId)).ToList();

        if (devices.Count == 0)
        {
            return;
        }

        bool dataDeletedSuccessfully = await DeleteDataByClientIdAsync(clientId, jwt);

        if (!dataDeletedSuccessfully)
        {
            _logger.LogError($"Failed to delete IoT data for client {clientId}.");
            throw new Exception("IoT data deletion failed. Device deletion aborted.");
        }

        foreach (var device in devices)
        {
            await _deviceRepo.RemoveAsync(device);
        }
    }

    // Helper method to delete IoT data by client ID
    private async Task<bool> DeleteDataByClientIdAsync(int clientId, string jwt)
    {
        try
        {
            using var httpClient = new HttpClient();
            httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", jwt);

            var response = await httpClient.DeleteAsync($"http://localhost:5099/api/data/client-id/{clientId}");

            if (!response.IsSuccessStatusCode)
            {
                _logger.LogError($"Failed to delete IoT data for client {clientId}. Status Code: {response.StatusCode}");
                return false;
            }

            return true;
        }
        catch (Exception ex)
        {
            _logger.LogError($"Error calling DataService to delete IoT data: {ex.Message}");
            return false;
        }
    }

    private async Task ValidateDevEui(string devEui, string name)
    {
        if (await _deviceRepo.IsThereAnyEntityWithSameDevEuiAsync(devEui))
        {
            throw new DevEuiDuplicationException(devEui);
        }
    }

    private async Task ValidateName(int clientId, string devEui, string name)
    {

        if (await _deviceRepo.IsThereAnyEntityWithSameNameForSameClientAsync(clientId, devEui, name))
        {
            throw new DeviceNameDuplicationException(name);
        }
    }


}