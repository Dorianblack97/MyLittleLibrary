using MongoDB.Driver;
using MyLittleLibrary.Domain;

namespace MyLittleLibrary.Infrastructure;

public class TestRepository
{
    private readonly IMongoCollection<BaseObject> _collection;

    public BaseObjectRepository(string connectionString, string databaseName)
    {
        var client = new MongoClient(connectionString);
        var database = client.GetDatabase(databaseName);
        _collection = database.GetCollection<BaseObject>(Contracts.MongoCollection);
    }

    // Get 8 most recent objects based on Timestamp
    public async Task<List<BaseObject>> GetMostRecentAsync(int count = 8)
    {
        return await _collection
            .Find(_ => true)
            .SortByDescending(o => o.Timestamp)
            .Limit(count)
            .ToListAsync();
    }

    // Get 8 most recent objects by collection type
    public async Task<List<BaseObject>> GetMostRecentByTypeAsync(Collection collectionType, int count = 8)
    {
        return await _collection
            .Find(o => o.CollectionType == collectionType)
            .SortByDescending(o => o.Timestamp)
            .Limit(count)
            .ToListAsync();
    }

    // Basic CRUD operations for BaseObject
    
    // Create
    public async Task<BaseObject> CreateAsync(BaseObject baseObject)
    {
        await _collection.InsertOneAsync(baseObject);
        return baseObject;
    }

    // Read - Get all
    public async Task<List<BaseObject>> GetAllAsync()
        => await _collection.Find(_ => true).ToListAsync();

    // Read - Get by ID
    public async Task<BaseObject> GetByIdAsync(string id)
        => await _collection.Find(o => o.Id == id).FirstOrDefaultAsync();

    // Delete
    public async Task<bool> DeleteAsync(string id)
    {
        var result = await _collection.DeleteOneAsync(o => o.Id == id);
        return result.IsAcknowledged && result.DeletedCount > 0;
    }
}
No newline at end of file