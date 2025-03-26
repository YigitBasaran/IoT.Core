namespace IoT.Core.CommonInfrastructure.Exception
{
    public class BaseException(string message, int statusCode) : System.Exception(message)
    {
        public int StatusCode { get; set; } = statusCode;
    }
}
