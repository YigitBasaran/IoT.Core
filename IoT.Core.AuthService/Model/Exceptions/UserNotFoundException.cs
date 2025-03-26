using IoT.Core.CommonInfrastructure.Exception;

namespace IoT.Core.AuthService.Model.Exceptions
{
    public class UserNotFoundException() : BaseException("User cannot be found.", 404)

    {
    }
}
