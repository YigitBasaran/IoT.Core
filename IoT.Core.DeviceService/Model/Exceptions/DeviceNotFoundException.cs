using IoT.Core.CommonInfrastructure.Exception;

namespace IoT.Core.DeviceService.Model.Exceptions
{
    public class DeviceNotFoundException(string devEui) : BaseException($"Device cannot be found. DevEui: {devEui}", 404)
    {
    }
}
