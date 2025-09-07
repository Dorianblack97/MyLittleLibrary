using Microsoft.Extensions.Options;
using MongoDB.Bson;
using MongoDB.Driver;
using MyLittleLibrary.Domain;
using MyLittleLibrary.Infrastructure.Constants;
using MyLittleLibrary.Infrastructure.Options;

namespace MyLittleLibrary.Infrastructure;

public class MangaRepository
{
    private readonly IMongoCollection<Book.Manga> _collection;

    public MangaRepository(IOptions<MongoOptions> options)
    {
        var client = new MongoClient(options.Value.ConnectionString);
        var database = client.GetDatabase(options.Value.DatabaseName);
        _collection = database.GetCollection<Book.Manga>(MongoDbContracts.MongoCollection);
    }

    // Create
    public async Task<Book.Manga> CreateAsync(Book.Manga manga, CancellationToken cancellationToken = default)
    {
        await _collection.InsertOneAsync(manga, cancellationToken: cancellationToken);
        return manga;
    }

    // Read - Get all
    public async Task<List<Book.Manga>> GetAllAsync(CancellationToken cancellationToken = default) 
        => await _collection.Find(m => m.CollectionType == Collection.Manga).ToListAsync(cancellationToken);

    // Read - Get all by title
    public async Task<List<Book.Manga>> GetAllByTitleAsync(string title, CancellationToken cancellationToken = default) 
        => await _collection.Find(m => m.Title == title && m.CollectionType == Collection.Manga).ToListAsync(cancellationToken);

    // Read - Get by ID
    public async Task<Book.Manga> GetByIdAsync(string id, CancellationToken cancellationToken = default) 
        => await _collection.Find(m => m.Id == id).FirstOrDefaultAsync(cancellationToken);

    // Read - Get by title
    public async Task<Book.Manga> GetByTitleAsync(string title, CancellationToken cancellationToken = default) 
        => await _collection.Find(m => m.Title == title && m.CollectionType == Collection.Manga).FirstOrDefaultAsync(cancellationToken);

    // Update
    public async Task<bool> UpdateAsync(string id, Book.Manga updatedManga, CancellationToken cancellationToken = default)
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
            update,
            cancellationToken: cancellationToken);

        return result.IsAcknowledged && result.ModifiedCount > 0;
    }

    // Delete
    public async Task<bool> DeleteAsync(string id, CancellationToken cancellationToken = default)
    {
        var result = await _collection.DeleteOneAsync(m => m.Id == id, cancellationToken);
        return result.IsAcknowledged && result.DeletedCount > 0;
    }

    // Search by title (partial match)
    public async Task<List<Book.Manga>> SearchByTitleAsync(string titleQuery, CancellationToken cancellationToken = default)
    {
        var filter = Builders<Book.Manga>.Filter.And(
            Builders<Book.Manga>.Filter.Regex(m => m.Title, new BsonRegularExpression(titleQuery, "i")),
            Builders<Book.Manga>.Filter.Eq(m => m.CollectionType, Collection.Manga)
        );
        return await _collection.Find(filter).ToListAsync(cancellationToken);
    }
}