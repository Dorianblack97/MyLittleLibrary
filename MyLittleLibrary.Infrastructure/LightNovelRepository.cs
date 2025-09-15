using Microsoft.Extensions.Options;
using MongoDB.Bson;
using MongoDB.Driver;
using MyLittleLibrary.Domain;
using MyLittleLibrary.Infrastructure.Constants;
using MyLittleLibrary.Infrastructure.Options;

using Microsoft.Extensions.Logging;

namespace MyLittleLibrary.Infrastructure;

public class LightNovelRepository
{
    private readonly IMongoCollection<Book.LightNovel> _collection;
    private readonly ILogger<LightNovelRepository> _logger;

    public LightNovelRepository(IOptions<MongoOptions> options, ILogger<LightNovelRepository> logger)
    {
        _logger = logger;
        var client = new MongoClient(options.Value.ConnectionString);
        var database = client.GetDatabase(options.Value.DatabaseName);
        _collection = database.GetCollection<Book.LightNovel>(MongoDbContracts.MongoCollection);
        _logger.LogInformation("LightNovelRepository initialized for {Db}", options.Value.DatabaseName);
    }

    // Create
    public async Task<Book.LightNovel> CreateAsync(Book.LightNovel LightNovel, CancellationToken cancellationToken = default)
    {
        _logger.LogInformation("Creating LN {Title} vol {Volume}", LightNovel.Title, LightNovel.Volume);
        await _collection.InsertOneAsync(LightNovel, cancellationToken: cancellationToken);
        _logger.LogInformation("Created LN {Id}", LightNovel.Id);
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
    {
        var novel = await _collection.Find(l => l.Id == id).FirstOrDefaultAsync(cancellationToken);
        if (novel is null) _logger.LogWarning("LightNovel {Id} not found", id);
        return novel;
    }

    // Read - Get by title
    public async Task<Book.LightNovel> GetByTitleAsync(string title, CancellationToken cancellationToken = default)
    {
        var novel = await _collection.Find(l => l.Title == title && l.CollectionType == Collection.LightNovel)
            .FirstOrDefaultAsync(cancellationToken);
        if (novel is null) _logger.LogWarning("LightNovel {Title} not found", title);
        return novel;       
    }

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

        _logger.LogInformation("Updating LN {Id} to title {Title} v{Volume}", id, updatedLightNovel.Title,
            updatedLightNovel.Volume);
        var result = await _collection.UpdateOneAsync(
            l=> l.Id == id,
            update,
            cancellationToken: cancellationToken);

        var ok = result.IsAcknowledged && result.ModifiedCount > 0;
        if (!ok) _logger.LogWarning("Failed to update LN {Id}", id);
        return ok;       
    }

    // Delete
    public async Task<bool> DeleteAsync(string id, CancellationToken cancellationToken = default)
    {
        _logger.LogInformation("Deleting LN {Id}", id);       
        var result = await _collection.DeleteOneAsync(l => l.Id == id, cancellationToken);
        var ok = result.IsAcknowledged && result.DeletedCount > 0;
        if (!ok) _logger.LogWarning("No LN deleted for {Id}", id);
        return ok;       
    }

    // Search by title (prefix, case-insensitive via collation). Uses index on Title.
    public async Task<List<Book.LightNovel>> SearchByTitleAsync(string titleQuery, CancellationToken cancellationToken = default)
    {
        if (string.IsNullOrWhiteSpace(titleQuery))
        {
            return await GetAllAsync(cancellationToken);
        }

        var prefix = titleQuery.Trim();
        var upperBound = prefix + "\uffff";

        var builder = Builders<Book.LightNovel>.Filter;
        var filter = builder.And(
            builder.Gte(l => l.Title, prefix),
            builder.Lt(l => l.Title, upperBound),
            builder.Eq(l => l.CollectionType, Collection.LightNovel)
        );

        var options = new FindOptions<Book.LightNovel>
        {
            Collation = new Collation("simple", strength: CollationStrength.Secondary)
        };

        using var cursor = await _collection.FindAsync(filter, options, cancellationToken);
        return await cursor.ToListAsync(cancellationToken);
    }
}