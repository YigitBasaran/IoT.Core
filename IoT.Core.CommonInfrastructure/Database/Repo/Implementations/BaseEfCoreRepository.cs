
using System.Linq.Expressions;

using Microsoft.EntityFrameworkCore;

namespace IoT.Core.CommonInfrastructure.Database.Repo.Implementations
{
    public class BaseEfCoreRepository<TEntity, TId, TContext> : IRepository<TEntity, TId>
        where TEntity : BaseEntity<TId>
        where TContext : DbContext
    {
        protected readonly TContext _context;
        protected readonly DbSet<TEntity> _dbSet;

        public BaseEfCoreRepository(TContext context)
        {
            _context = context;
            _dbSet = context.Set<TEntity>();
        }

        public async Task<TEntity?> FindByIdAsync(TId id)
        {
            return await _dbSet.FindAsync(id);
        }

        public async Task<IEnumerable<TEntity>> GetAllAsync()
        {
            return await _dbSet.ToListAsync();
        }

        public async Task AddAsync(TEntity entity)
        {
            await _dbSet.AddAsync(entity);
            await _context.SaveChangesAsync();
        }

        public async Task UpdateAsync(TEntity entity)
        {
            _dbSet.Update(entity);
            await _context.SaveChangesAsync();
        }

        public async Task RemoveAsync(TEntity entity)
        {
            _dbSet.Remove(entity);
            await _context.SaveChangesAsync();
        }

        public async Task<IEnumerable<TEntity>> FindAsync(Expression<Func<TEntity, bool>> predicate)
        {
            return await _dbSet.Where(predicate).ToListAsync();
        }
    }
}
