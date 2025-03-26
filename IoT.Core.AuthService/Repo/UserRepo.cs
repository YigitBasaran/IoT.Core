using IoT.Core.AuthService.Model;
using IoT.Core.AuthService.Repo.DbContext;
using IoT.Core.CommonInfrastructure.Database.Repo.Implementations;

namespace IoT.Core.AuthService.Repo
{
    public class UserRepo : BaseEfCoreRepository<User, int, UserDbContext>, IUserRepo
    {
        public UserRepo(UserDbContext context) : base(context) { }
    }
}
