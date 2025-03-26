using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;
using MongoDB.Driver;

namespace IoT.Core.CommonInfrastructure.Database.Repo.Implementations
{
    public class BaseMongoRepository<TEntity, TId> : IRepository<TEntity, TId> where TEntity : BaseEntity<TId>
    {
        protected readonly IMongoCollection<TEntity> _collection;

        public BaseMongoRepository(IMongoClient mongoClient, string databaseName, string collectionName)
        {
            var database = mongoClient.GetDatabase(databaseName);
            _collection = database.GetCollection<TEntity>(collectionName);
        }

        public async Task<TEntity?> FindByIdAsync(TId id)
        {
            return await _collection.Find(e => e.Id.Equals(id)).FirstOrDefaultAsync();
        }

        public async Task<IEnumerable<TEntity>> GetAllAsync()
        {
            return await _collection.Find(_ => true).ToListAsync();
        }

        public async Task AddAsync(TEntity entity)
        {
            await _collection.InsertOneAsync(entity);
        }

        public async Task UpdateAsync(TEntity entity)
        {
            await _collection.ReplaceOneAsync(e => e.Id.Equals(entity.Id), entity);
        }

        public async Task RemoveAsync(TEntity entity)
        {
            await _collection.DeleteOneAsync(e => e.Id.Equals(entity.Id));
        }

        public async Task<IEnumerable<TEntity>> FindAsync(Expression<Func<TEntity, bool>> predicate)
        {
            return await _collection.Find(predicate).ToListAsync();
        }
    }
}
