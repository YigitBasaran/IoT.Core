using IoT.Core.CommonInfrastructure.Database.Repo;
using IoT.Core.DataService.Model;

namespace IoT.Core.DataService.Repo
{
    public interface IDataRepo : IRepository<Data, Guid>
    {
        public Task AddBulkDataAsync(List<Data> dataList);
        public Task DeleteBulkDataByDevEuiAsync(string devEui);
        public Task DeleteBulkDataByClientIdAsync(int clientId);
    }
}
