using MongoDB.Driver;
using Play.Common.Interfaces;
using System.Linq.Expressions;
namespace Play.Common.MongoDb
{
    public class MongoRepository<T> : IRepository<T> where T : IEntity
    {
        private readonly IMongoCollection<T> dbCollection;
        private readonly FilterDefinitionBuilder<T> filterBuilder = Builders<T>.Filter;

        public MongoRepository(IMongoDatabase database, string collectionName)
        {
            dbCollection = database.GetCollection<T>(collectionName);
        }

        public async Task<IReadOnlyCollection<T>> GetAllAsync()
        {
            return await dbCollection.Find(filterBuilder.Empty).ToListAsync();
        }

        public async Task<IReadOnlyCollection<T>> GetAllAsync(Expression<Func<T, bool>> filter)
        {
            return await dbCollection.Find(filter).ToListAsync();
        }

        public async Task<T> GetAsync(Guid id)
        {
            FilterDefinition<T> filter = filterBuilder.Eq(item => item.Id, id);
            return await dbCollection.Find(filter).SingleOrDefaultAsync();
        }

        public async Task<T> GetAsync(Expression<Func<T, bool>> filter)
        {
            return await dbCollection.Find(filter).FirstOrDefaultAsync();
        }

        public async Task CreateAsync(T item)
        {
            await dbCollection.InsertOneAsync(item);
        }

        public async Task DeleteAsync(Guid id)
        {
            FilterDefinition<T> filter = filterBuilder.Eq(item => item.Id, id);
            await dbCollection.DeleteOneAsync(filter);
        }

        public async Task UpdateAsync(T item)
        {
            FilterDefinition<T> filter = filterBuilder.Eq(existingItem => existingItem.Id, item.Id);
            await dbCollection.ReplaceOneAsync(filter, item);
        }             
    }
}
