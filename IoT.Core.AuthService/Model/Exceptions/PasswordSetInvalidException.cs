using IoT.Core.CommonInfrastructure.Exception;

namespace IoT.Core.AuthService.Model.Exceptions
{
    public class PasswordSetInvalidException() : BaseException("Password set process is invalid.", 400)

    {
    }
}
