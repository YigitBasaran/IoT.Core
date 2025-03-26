using IoT.Core.CommonInfrastructure.Exception;

namespace IoT.Core.DeviceService.Model.Exceptions
{
    public class DeviceNameDuplicationException(string deviceName) : BaseException($"Device Name {deviceName} is already in use.", 500)
    {
    }
}
