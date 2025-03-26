using IoT.Core.AuthService.Model;
using IoT.Core.ClientService.Model;

namespace IoT.Core.ClientService.Service;

public interface IClientService
{
    Task<List<Client>> GetAllClientsAsync();
    Task<Client?> GetClientByIdAsync(int id);
    Task<Client> CreateClientAsync(string name, RoleEnum role, string email);
    Task UpdateClientEmailAsync(int id, string email);
    Task DeleteClientAsync(int id, string jwt);
}