using Microsoft.Extensions.Options;
using MongoDB.Bson;
using MongoDB.Driver;
using MyLittleLibrary.Domain;
using MyLittleLibrary.Infrastructure.Constants;
using MyLittleLibrary.Infrastructure.Options;

namespace MyLittleLibrary.Infrastructure;

public class LightNovelRepository
{
    private readonly IMongoCollection<Book.LightNovel> _collection;

    public LightNovelRepository(IOptions<MongoOptions> options)
    {
        var client = new MongoClient(options.Value.ConnectionString);
        var database = client.GetDatabase(options.Value.DatabaseName);
        _collection = database.GetCollection<Book.LightNovel>(MongoDbContracts.MongoCollection);
    }

    // Create
    public async Task<Book.LightNovel> CreateAsync(Book.LightNovel LightNovel, CancellationToken cancellationToken = default)
    {
        await _collection.InsertOneAsync(LightNovel, cancellationToken: cancellationToken);
        return LightNovel;
    }

    // Read - Get all
    public async Task<List<Book.LightNovel>> GetAllAsync(CancellationToken cancellationToken = default) 
        => await _collection.Find(l => l.CollectionType == Collection.LightNovel).ToListAsync(cancellationToken);

    // Read - Get all by title
    public async Task<List<Book.LightNovel>> GetAllByTitleAsync(string title, CancellationToken cancellationToken = default) 
        => await _collection.Find(l => l.Title == title && l.CollectionType == Collection.LightNovel).ToListAsync(cancellationToken);

    // Read - Get by ID
    public async Task<Book.LightNovel> GetByIdAsync(string id, CancellationToken cancellationToken = default) 
        => await _collection.Find(l => l.Id == id).FirstOrDefaultAsync(cancellationToken);

    // Read - Get by title
    public async Task<Book.LightNovel> GetByTitleAsync(string title, CancellationToken cancellationToken = default) 
        => await _collection.Find(l => l.Title == title && l.CollectionType == Collection.LightNovel).FirstOrDefaultAsync(cancellationToken);

    // Update
    public async Task<bool> UpdateAsync(string id, Book.LightNovel updatedLightNovel, CancellationToken cancellationToken = default)
    {
        var update = Builders<Book.LightNovel>.Update
            .Set(l => l.Title, updatedLightNovel.Title)
            .Set(l => l.TitleSlug, updatedLightNovel.TitleSlug)
            .Set(l => l.Author, updatedLightNovel.Author)
            .Set(l => l.Illustrator, updatedLightNovel.Illustrator)
            .Set(l => l.Volume, updatedLightNovel.Volume)
            .Set(l => l.ImagePath, updatedLightNovel.ImagePath)
            .Set(l => l.IsDigital, updatedLightNovel.IsDigital)
            .Set(l => l.IsRead, updatedLightNovel.IsRead)
            .Set(l => l.PublishDate, updatedLightNovel.PublishDate);

        var result = await _collection.UpdateOneAsync(
            l=> l.Id == id,
            update,
            cancellationToken: cancellationToken);

        return result.IsAcknowledged && result.ModifiedCount > 0;
    }

    // Delete
    public async Task<bool> DeleteAsync(string id, CancellationToken cancellationToken = default)
    {
        var result = await _collection.DeleteOneAsync(l => l.Id == id, cancellationToken);
        return result.IsAcknowledged && result.DeletedCount > 0;
    }

    // Search by title (partial match)
    public async Task<List<Book.LightNovel>> SearchByTitleAsync(string titleQuery, CancellationToken cancellationToken = default)
    {
        var filter = Builders<Book.LightNovel>.Filter.And(
            Builders<Book.LightNovel>.Filter.Regex(l => l.Title, new BsonRegularExpression(titleQuery, "i")),
            Builders<Book.LightNovel>.Filter.Eq(l => l.CollectionType, Collection.LightNovel)
        );
        return await _collection.Find(filter).ToListAsync(cancellationToken);
    }
}