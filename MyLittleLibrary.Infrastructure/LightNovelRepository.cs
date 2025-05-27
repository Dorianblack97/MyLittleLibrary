using MongoDB.Bson;
using MongoDB.Driver;
using MyLittleLibrary.Domain;

namespace MyLittleLibrary.Infrastructure;

public class LightNovelRepository
{
    private readonly IMongoCollection<LightNovel> _collection;

    public LightNovelRepository(string connectionString, string databaseName)
    {
        var client = new MongoClient(connectionString);
        var database = client.GetDatabase(databaseName);
        _collection = database.GetCollection<LightNovel>("LightNovels");
    }

    // Create
    public async Task<LightNovel> CreateAsync(LightNovel LightNovel)
    {
        await _collection.InsertOneAsync(LightNovel);
        return LightNovel;
    }

    // Read - Get all
    public async Task<List<LightNovel>> GetAllAsync()
    {
        return await _collection.Find(_ => true).ToListAsync();
    }
    
    // Read - Get all by title
    public async Task<List<LightNovel>> GetAllByTitleAsync(string title)
    {
        return await _collection.Find(m => m.Title == title).ToListAsync();
    }

    // Read - Get by ID
    public async Task<LightNovel> GetByIdAsync(string id)
    {
        return await _collection.Find(m => m.Id == id).FirstOrDefaultAsync();
    }

    // Read - Get by title
    public async Task<LightNovel> GetByTitleAsync(string title)
    {
        return await _collection.Find(m => m.Title == title).FirstOrDefaultAsync();
    }

    // Update
    public async Task<bool> UpdateAsync(string id, LightNovel updatedLightNovel)
    {
        var update = Builders<LightNovel>.Update
            .Set(m => m.Title, updatedLightNovel.Title)
            .Set(m => m.TitleSlug, updatedLightNovel.TitleSlug)
            .Set(m => m.Author, updatedLightNovel.Author)
            .Set(m => m.Illustrator, updatedLightNovel.Illustrator)
            .Set(m => m.Volume, updatedLightNovel.Volume)
            .Set(m => m.ImagePath, updatedLightNovel.ImagePath)
            .Set(m => m.IsDigital, updatedLightNovel.IsDigital)
            .Set(m => m.IsRead, updatedLightNovel.IsRead)
            .Set(m => m.PublishDate, updatedLightNovel.PublishDate);
    
        var result = await _collection.UpdateOneAsync(
            m => m.Id == id,
            update);

        return result.IsAcknowledged && result.ModifiedCount > 0;
    }

    // Delete
    public async Task<bool> DeleteAsync(string id)
    {
        var result = await _collection.DeleteOneAsync(m => m.Id == id);
        return result.IsAcknowledged && result.DeletedCount > 0;
    }

    // Search by title (partial match)
    public async Task<List<LightNovel>> SearchByTitleAsync(string titleQuery)
    {
        var filter = Builders<LightNovel>.Filter.Regex(m => m.Title, new BsonRegularExpression(titleQuery, "i"));
        return await _collection.Find(filter).ToListAsync();
    }
}