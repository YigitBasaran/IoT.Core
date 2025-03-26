using IoT.Core.DataService.Controllers.Dto;
using IoT.Core.DataService.Model;

namespace IoT.Core.DataService.Service
{
    public interface IDataService
    {
        Task<List<Data>> GetDataByDevEuiBetweenTimespanAsync(string devEui, DateTime startDateTime, DateTime endDateTime);
        Task<List<Data>> GetDataByClientIdBetweenTimespanAsync(int clientId, DateTime startDateTime, DateTime endDateTime);
        Task<List<Data>> CreateBulkDataAsync(AddDataRequestDto addDataRequestDto);
        Task DeleteDataByDevEuiAsync(string devEui);
        Task DeleteDataByClientIdAsync(int clientId);
    }
}
