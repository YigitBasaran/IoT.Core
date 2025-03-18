using IoT.Core.DeviceService.Shared.Exception;

namespace IoT.Core.DeviceService.Model.Exceptions
{
    public class DeviceNotFoundException(Guid deviceId) : BaseException($"Device cannot be found. Device Id: {deviceId}", 404)
    {
    }
}
