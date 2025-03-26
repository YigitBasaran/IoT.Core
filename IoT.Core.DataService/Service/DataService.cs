using IoT.Core.DataService.Model;
using IoT.Core.DataService.Repo;
using IoT.Core.DataService.Controllers.Dto;
using System.Text.Json;
using IoT.Core.DataService.Service.Dto;

namespace IoT.Core.DataService.Service
{
    public class DataService : IDataService
    {
        private readonly IDataRepo _dataRepo;
        private readonly HttpClient _httpClient;
        private readonly ILogger<DataService> _logger;

        public DataService(IDataRepo dataRepo, IHttpClientFactory httpClientFactory, ILogger<DataService> logger)
        {
            _dataRepo = dataRepo;
            _httpClient = httpClientFactory.CreateClient(nameof(DataService));
            _logger = logger;
        }

        public async Task<List<Data>> GetDataByDevEuiBetweenTimespanAsync(string devEui, DateTime startDateTime, DateTime endDateTime)
        {
            startDateTime = DateTime.SpecifyKind(startDateTime, DateTimeKind.Utc);
            endDateTime = DateTime.SpecifyKind(endDateTime, DateTimeKind.Utc);
            return (await _dataRepo.FindAsync(d => d.DevEui == devEui && d.CreatedAt >= startDateTime && d.CreatedAt <= endDateTime)).ToList();
        }

        public async Task<List<Data>> GetDataByClientIdBetweenTimespanAsync(int clientId, DateTime startDateTime, DateTime endDateTime)
        {
            startDateTime = DateTime.SpecifyKind(startDateTime, DateTimeKind.Utc);
            endDateTime = DateTime.SpecifyKind(endDateTime, DateTimeKind.Utc);
            return (await _dataRepo.FindAsync(d => d.ClientId == clientId && d.CreatedAt >= startDateTime && d.CreatedAt <= endDateTime)).ToList();
        }

        public async Task<List<Data>> CreateBulkDataAsync(AddDataRequestDto addDataRequestDto)
        {
            var uniqueDevEuis = addDataRequestDto.DataRequestDto.Select(d => d.DevEui).Distinct().ToList();

            var devEuiToClientIdMap = await GetClientIdsForDevEuisAsync(uniqueDevEuis);

            var validDevEuis = devEuiToClientIdMap.Keys.ToHashSet();

            var createdDataList = new List<Data>();
            foreach (var request in addDataRequestDto.DataRequestDto)
            {
                if (validDevEuis.Contains(request.DevEui))
                {
                    var data = Data.OnCreate(devEuiToClientIdMap[request.DevEui], request.DevEui, request.Payload);
                    createdDataList.Add(data);
                }
                else
                {
                    _logger.LogWarning($"Skipping data for DevEui {request.DevEui} because ClientId lookup failed.");
                }
            }

            // Save only valid records in bulk
            if (createdDataList.Count > 0)
            {
                await _dataRepo.AddBulkDataAsync(createdDataList);
            }
            return createdDataList;
        }

        public async Task DeleteDataByDevEuiAsync(string devEui)
        {
            await _dataRepo.DeleteBulkDataByDevEuiAsync(devEui);
        }

        public async Task DeleteDataByClientIdAsync(int clientId)
        {
            await _dataRepo.DeleteBulkDataByClientIdAsync(clientId);
        }

        private async Task<Dictionary<string, int>> GetClientIdsForDevEuisAsync(List<string> devEuis)
        {
            var devEuiToClientId = new Dictionary<string, int>();

            foreach (var devEui in devEuis)
            {
                try
                {
                    var response = await _httpClient.GetAsync($"http://localhost:5097/api/device/{devEui}");

                    if (!response.IsSuccessStatusCode)
                    {
                        throw new Exception($"DeviceService returned {response.StatusCode}");
                    }

                    // Deserialize into a locally defined DTO
                    var deviceResponse = await JsonSerializer.DeserializeAsync<DeviceResponseDto>(
                        await response.Content.ReadAsStreamAsync(),
                        new JsonSerializerOptions { PropertyNameCaseInsensitive = true });

                    if (deviceResponse != null) devEuiToClientId[devEui] = deviceResponse.ClientId;
                    else
                    {
                        _logger.LogWarning($"Failed to deserialize response.");
                    }
                }
                catch (Exception ex)
                {
                    _logger.LogError($"Failed to fetch ClientId for DevEui {devEui}: {ex.Message}");
                }
            }

            return devEuiToClientId;
        }
    }
}

