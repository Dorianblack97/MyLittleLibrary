using MongoDB.Bson;
using MongoDB.Driver;
using MyLittleLibrary.Domain;

namespace MyLittleLibrary.Infrastructure;

public class MangaRepository
{
    private readonly IMongoCollection<Manga> _collection;

    public MangaRepository(string connectionString, string databaseName)
    {
        var client = new MongoClient(connectionString);
        var database = client.GetDatabase(databaseName);
        _collection = database.GetCollection<Manga>("Mangas");
    }

    // Create
    public async Task<Manga> CreateAsync(Manga manga)
    {
        await _collection.InsertOneAsync(manga);
        return manga;
    }

    // Read - Get all
    public async Task<List<Manga>> GetAllAsync()
    {
        return await _collection.Find(_ => true).ToListAsync();
    }
    
    // Read - Get all by title
    public async Task<List<Manga>> GetAllByTitleAsync(string title)
    {
        return await _collection.Find(m => m.Title == title).ToListAsync();
    }

    // Read - Get by ID
    public async Task<Manga> GetByIdAsync(string id)
    {
        return await _collection.Find(m => m.Id == id).FirstOrDefaultAsync();
    }

    // Read - Get by title
    public async Task<Manga> GetByTitleAsync(string title)
    {
        return await _collection.Find(m => m.Title == title).FirstOrDefaultAsync();
    }

    // Update
    public async Task<bool> UpdateAsync(string id, Manga updatedManga)
    {
        var result = await _collection.ReplaceOneAsync(
            m => m.Id == id,
            updatedManga);

        return result.IsAcknowledged && result.ModifiedCount > 0;
    }

    // Delete
    public async Task<bool> DeleteAsync(string id)
    {
        var result = await _collection.DeleteOneAsync(m => m.Id == id);
        return result.IsAcknowledged && result.DeletedCount > 0;
    }

    // Search by title (partial match)
    public async Task<List<Manga>> SearchByTitleAsync(string titleQuery)
    {
        var filter = Builders<Manga>.Filter.Regex(m => m.Title, new BsonRegularExpression(titleQuery, "i"));
        return await _collection.Find(filter).ToListAsync();
    }
}