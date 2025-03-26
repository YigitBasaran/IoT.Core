using IoT.Core.CommonInfrastructure.Exception;

namespace IoT.Core.ClientService.Model.Exceptions
{
    public class ClientNameDuplicationException(string name) : BaseException($"Client Name {name} is already in use.", 500)
    {
    }
}
