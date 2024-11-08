using System.Linq.Expressions;
using MongoDB.Driver;

namespace Berrevoets.Play.Economy.Common.MongoDB;

public class MongoRepository<T> : IRepository<T> where T : IEntity
{
    private readonly IMongoCollection<T>        _dbCollection;
    private readonly FilterDefinitionBuilder<T> _filterBuilder = Builders<T>.Filter;

    public MongoRepository(IMongoDatabase database, string collectionName)
    {
        _dbCollection = database.GetCollection<T>(collectionName);
    }

    public async Task<IReadOnlyCollection<T>> GetAllAsync()
    {
        return await _dbCollection.Find(_filterBuilder.Empty).ToListAsync();
    }

    public async Task<IReadOnlyCollection<T>> GetAllAsync(Expression<Func<T, bool>> filter)
    {
        return await _dbCollection.Find(filter).ToListAsync();
    }

    public async Task<T> GetAsync(Guid id)
    {
        var filter = _filterBuilder.Eq(entity => entity.Id, id);
        return await _dbCollection.Find(filter).SingleOrDefaultAsync();
    }

    public async Task<T> GetAsync(Expression<Func<T, bool>> filter)
    {
        return await _dbCollection.Find(filter).SingleOrDefaultAsync();
    }

    public async Task CreateAsync(T item)
    {
        if (item is null)
        {
            throw new ArgumentNullException(nameof(item));
        }

        await _dbCollection.InsertOneAsync(item);
    }

    public async Task UpdateAsync(T item)
    {
        if (item is null)
        {
            throw new ArgumentNullException(nameof(item));
        }

        var filter = _filterBuilder.Eq(existingEntity => existingEntity.Id, item.Id);
        await _dbCollection.ReplaceOneAsync(filter, item);
    }

    public async Task DeleteAsync(Guid id)
    {
        var filter = _filterBuilder.Eq(entity => entity.Id, id);
        await _dbCollection.DeleteOneAsync(filter);
    }
}