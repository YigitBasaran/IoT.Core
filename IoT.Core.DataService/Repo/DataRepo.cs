using IoT.Core.CommonInfrastructure.Database.Repo.Implementations;
using IoT.Core.DataService.Model;
using IoT.Core.DataService.Repo.DbContext;
using Microsoft.EntityFrameworkCore;

namespace IoT.Core.DataService.Repo
{
    public class DataRepo : BaseEfCoreRepository<Data, Guid, DataDbContext>, IDataRepo
    {
        public DataRepo(DataDbContext context) : base(context) { }
        public async Task AddBulkDataAsync(List<Data> dataList)
        {
            if (dataList == null || dataList.Count == 0)
            {
                return;
            }

            try
            {
                await _dbSet.AddRangeAsync(dataList);
                await _context.SaveChangesAsync();
            }
            catch (Exception ex)
            {
                throw new Exception("Error occurred during bulk insert data", ex);
            }
        }

        public async Task DeleteBulkDataByDevEuiAsync(string devEui)
        {
            try
            {
                var dataRecords = await _dbSet.Where(d => d.DevEui == devEui).ToListAsync();
                if (dataRecords.Count > 0)
                {
                    _dbSet.RemoveRange(dataRecords);
                    await _context.SaveChangesAsync();
                }
            }
            catch (Exception ex)
            {
                throw new Exception($"Error occurred while deleting data for DevEui {devEui}", ex);
            }
        }

        public async Task DeleteBulkDataByClientIdAsync(int clientId)
        {
            try
            {
                var dataRecords = await _dbSet.Where(d => d.ClientId == clientId).ToListAsync();
                if (dataRecords.Count > 0)
                {
                    _dbSet.RemoveRange(dataRecords);
                    await _context.SaveChangesAsync();
                }
            }
            catch (Exception ex)
            {
                throw new Exception($"Error occurred while deleting data for ClientId {clientId}", ex);
            }
        }
    }
}
