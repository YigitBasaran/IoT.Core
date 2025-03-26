using IoT.Core.CommonInfrastructure.Exception;

namespace IoT.Core.AuthService.Model.Exceptions
{
    public class PasswordUpdateInvalidException() : BaseException("Password cannot be updated. Check username and/or old password.", 400)

    {
    }
}
