using IoT.Core.DeviceService.Shared;
using IoT.Core.DeviceService.Shared.Exception;

namespace IoT.Core.DeviceService.Model.Exceptions
{
    public class DeviceNameDuplicationException(string deviceName) : BaseException($"Device Name {deviceName} is already in use.", 500)
    {
    }
}
