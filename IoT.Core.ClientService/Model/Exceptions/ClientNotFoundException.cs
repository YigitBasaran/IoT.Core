using IoT.Core.CommonInfrastructure.Exception;

namespace IoT.Core.ClientService.Model.Exceptions
{
    public class ClientNotFoundException(int id) : BaseException($"Client with Id {id} cannot be found.", 404)
    {
    }
}
