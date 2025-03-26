using IoT.Core.ClientService.Model;
using IoT.Core.CommonInfrastructure.Database.Repo;

namespace IoT.Core.ClientService.Repo
{
    public interface IClientRepo : IRepository<Client, int>
    {
        public Task<bool> IsNameUniqueAsync(string name, int? excludedId = null);
    }

}
