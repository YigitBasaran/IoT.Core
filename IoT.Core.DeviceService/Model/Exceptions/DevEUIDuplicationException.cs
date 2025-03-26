using IoT.Core.CommonInfrastructure.Exception;
namespace IoT.Core.DeviceService.Model.Exceptions
{
    public class DevEuiDuplicationException(string devEui) : BaseException($"DevEUI {devEui} is already in use.", 500)
    {
    }
}
