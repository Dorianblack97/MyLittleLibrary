using MongoDB.Bson;
using MongoDB.Driver;
using MyLittleLibrary.Domain;

namespace MyLittleLibrary.Infrastructure;

public class MangaRepository
{
    private readonly IMongoCollection<Book.Manga> _collection;

    public MangaRepository(string connectionString, string databaseName)
    {
        var client = new MongoClient(connectionString);
        var database = client.GetDatabase(databaseName);
        _collection = database.GetCollection<Book.Manga>("Mangas");
    }

    // Create
    public async Task<Book.Manga> CreateAsync(Book.Manga manga)
    {
        await _collection.InsertOneAsync(manga);
        return manga;
    }

    // Read - Get all
    public async Task<List<Book.Manga>> GetAllAsync()
    {
        return await _collection.Find(_ => true).ToListAsync();
    }
    
    // Read - Get all by title
    public async Task<List<Book.Manga>> GetAllByTitleAsync(string title)
    {
        return await _collection.Find(m => m.Title == title).ToListAsync();
    }

    // Read - Get by ID
    public async Task<Book.Manga> GetByIdAsync(string id)
    {
        return await _collection.Find(m => m.Id == id).FirstOrDefaultAsync();
    }

    // Read - Get by title
    public async Task<Book.Manga> GetByTitleAsync(string title)
    {
        return await _collection.Find(m => m.Title == title).FirstOrDefaultAsync();
    }

    // Update
    public async Task<bool> UpdateAsync(string id, Book.Manga updatedManga)
    {
        var update = Builders<Book.Manga>.Update
            .Set(m => m.Title, updatedManga.Title)
            .Set(m => m.TitleSlug, updatedManga.TitleSlug)
            .Set(m => m.Author, updatedManga.Author)
            .Set(m => m.Illustrator, updatedManga.Illustrator)
            .Set(m => m.Volume, updatedManga.Volume)
            .Set(m => m.ImagePath, updatedManga.ImagePath)
            .Set(m => m.IsDigital, updatedManga.IsDigital)
            .Set(m => m.IsRead, updatedManga.IsRead)
            .Set(m => m.PublishDate, updatedManga.PublishDate);
    
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
    public async Task<List<Book.Manga>> SearchByTitleAsync(string titleQuery)
    {
        var filter = Builders<Book.Manga>.Filter.Regex(m => m.Title, new BsonRegularExpression(titleQuery, "i"));
        return await _collection.Find(filter).ToListAsync();
    }
}