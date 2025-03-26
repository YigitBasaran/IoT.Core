using IoT.Core.AuthService.Model;
using IoT.Core.CommonInfrastructure.Database.Repo;

namespace IoT.Core.AuthService.Repo
{
    public interface IUserRepo : IRepository<User, int>
    {
    }
}
