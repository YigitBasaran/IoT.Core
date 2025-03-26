using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;

namespace IoT.Core.CommonInfrastructure.Database.Repo
{
    public interface IRepository<TEntity, in TId> where TEntity : BaseEntity<TId>
    {
        Task<TEntity?> FindByIdAsync(TId id);
        Task<IEnumerable<TEntity>> GetAllAsync();
        Task AddAsync(TEntity entity);
        Task UpdateAsync(TEntity entity);
        Task RemoveAsync(TEntity entity);
        Task<IEnumerable<TEntity>> FindAsync(Expression<Func<TEntity, bool>> predicate);
    }
}
