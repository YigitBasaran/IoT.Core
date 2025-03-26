using IoT.Core.ClientService.Model;
using IoT.Core.ClientService.Model.Exceptions;
using IoT.Core.ClientService.Repo;
using IoT.Core.CommonInfrastructure.EventBus.Publisher;
using System.Net.Http.Headers;
using IoT.Core.AuthService.EventConsumer.ConsumedEvent;
using IoT.Core.AuthService.Model;

namespace IoT.Core.ClientService.Service;

public class ClientService : IClientService
{
    private readonly IClientRepo _clientRepo;
    private readonly HttpClient _httpClient;
    private readonly ILogger<ClientService> _logger;
    private readonly IEventPublisher _publisher;



    public ClientService(IClientRepo clientRepo, IHttpClientFactory httpClientFactory, ILogger<ClientService> logger, IEventPublisher publisher)
    {
        this._clientRepo = clientRepo;
        _httpClient = httpClientFactory.CreateClient();
        _logger = logger;
        _publisher = publisher;
    }


    public async Task<List<Client>> GetAllClientsAsync()
    {
        return (await _clientRepo.GetAllAsync()).ToList();
    }

    public async Task<Client?> GetClientByIdAsync(int id)
    {
        var client = await FindClientByIdAsync(id);

        return client;
    }

    public async Task<Client> CreateClientAsync(string name, RoleEnum role, string email)
    {
        await ValidateName(name);

        var client = Client.OnCreate(name, email);
        await _clientRepo.AddAsync(client);
        await PublishClientAddedEventAsync(client, role);
        return client;
    }

    public async Task UpdateClientEmailAsync(int id, string email)
    {
        var existingClient = await FindClientByIdAsync(id);
        if (!existingClient.Name.Equals(email))
        {
            existingClient.OnUpdateEmail(email);
            await _clientRepo.UpdateAsync(existingClient);
        }
    }

    public async Task DeleteClientAsync(int id, string jwt)
    {
        var client = await FindClientByIdAsync(id);

        await MarkClientAsDeletedAsync(client);

        bool devicesDeletedSuccessfully = await DeleteDevicesForClientAsync(id, jwt);

        if (!devicesDeletedSuccessfully)
        {
            await RollbackClientDeletionAsync(client);
            throw new Exception("Device deletion failed. Client deletion rolled back.");
        }

        await FullyDeleteClientAsync(client);

        await PublishClientDeletedEventAsync(client);
    }

    private async Task<Client> FindClientByIdAsync(int id)
    {
        var client = await _clientRepo.FindByIdAsync(id);
        if (client == null)
        {
            throw new ClientNotFoundException(id);
        }
        return client;
    }

    private async Task MarkClientAsDeletedAsync(Client client)
    {
        client.MarkAsDeleted();
        await _clientRepo.UpdateAsync(client);
    }

    private async Task<bool> DeleteDevicesForClientAsync(int clientId, string jwt)
    {
        try
        {
            using var httpClient = new HttpClient();
            httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", jwt);

            var response = await httpClient.DeleteAsync($"http://localhost:5097/api/device/delete/client-id/{clientId}");

            if (!response.IsSuccessStatusCode)
            {
                _logger.LogError($"Failed to delete devices for client {clientId}.");
                return false;
            }

            return true;
        }
        catch (Exception ex)
        {
            _logger.LogError($"Error calling DeviceService to delete devices: {ex.Message}");
            return false;
        }
    }

    private async Task RollbackClientDeletionAsync(Client client)
    {
        client.MarkAsUndeleted();
        await _clientRepo.UpdateAsync(client);
    }

    private async Task FullyDeleteClientAsync(Client client)
    {
        await _clientRepo.RemoveAsync(client);
    }

    private async Task PublishClientDeletedEventAsync(Client client)
    {
        var clientDeletedEvent = new ClientDeletedEvent(client.Id);
        await _publisher.PublishAsync("auth_service_exchange", "auth.client.deleted", clientDeletedEvent);
    }

    private async Task PublishClientAddedEventAsync(Client client, RoleEnum role)
    {
        var clientAddedEvent = new ClientAddedEvent(client.Id, client.Name, role);
        await _publisher.PublishAsync("auth_service_exchange", "auth.client.added", clientAddedEvent);
    }
    private async Task ValidateName(string name, int? excludedId = null)
    {
        bool isUnique = await _clientRepo.IsNameUniqueAsync(name);
        if (!isUnique)
        {
            throw new ClientNameDuplicationException(name);
        }
    }
}