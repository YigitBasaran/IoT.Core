using System.Runtime.InteropServices;

namespace IoT.Core.DeviceService.Shared.Exception
{
    public class BaseException(string message, int statusCode) : System.Exception(message)
    {
        public int StatusCode { get; set; } = statusCode;
    }
}
