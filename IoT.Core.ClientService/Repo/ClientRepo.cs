using IoT.Core.ClientService.Model;
using IoT.Core.ClientService.Repo.DbContext;
using IoT.Core.CommonInfrastructure.Database.Repo.Implementations;
using Microsoft.EntityFrameworkCore;

namespace IoT.Core.ClientService.Repo
{
    public class ClientRepo : BaseEfCoreRepository<Client, int, ClientDbContext>, IClientRepo
    {
        public ClientRepo(ClientDbContext context) : base(context) { }

        public async Task<bool> IsNameUniqueAsync(string name, int? excludedId = null)
        {
            return !await _context.Clients.AnyAsync(c => c.Name == name && (excludedId == null || c.Id != excludedId));
        }
    }
}
