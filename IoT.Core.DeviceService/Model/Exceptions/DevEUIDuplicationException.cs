using IoT.Core.DeviceService.Shared;
using IoT.Core.DeviceService.Shared.Exception;

namespace IoT.Core.DeviceService.Model.Exceptions
{
    public class DevEuiDuplicationException(string devEui) : BaseException($"DevEUI {devEui} is already in use.", 500)
    {
    }
}
