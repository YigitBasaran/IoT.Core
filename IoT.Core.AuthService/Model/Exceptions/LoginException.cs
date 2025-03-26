using IoT.Core.CommonInfrastructure.Exception;

namespace IoT.Core.AuthService.Model.Exceptions
{
    public class LoginException() : BaseException("Login process is not successful.", 400)

    {
    }
}
