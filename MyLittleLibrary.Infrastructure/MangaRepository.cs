using Microsoft.Extensions.Options;
using MongoDB.Bson;
using MongoDB.Driver;
using MyLittleLibrary.Domain;
using MyLittleLibrary.Infrastructure.Constants;
using MyLittleLibrary.Infrastructure.Options;
using Microsoft.Extensions.Logging;

namespace MyLittleLibrary.Infrastructure;

public class MangaRepository
{
    private readonly IMongoCollection<Book.Manga> _collection;

    private readonly ILogger<MangaRepository> _logger;

    public MangaRepository(IOptions<MongoOptions> options, ILogger<MangaRepository> logger)
    {
        _logger = logger;
        var client = new MongoClient(options.Value.ConnectionString);
        var database = client.GetDatabase(options.Value.DatabaseName);
        _collection = database.GetCollection<Book.Manga>(MongoDbContracts.MongoCollection);
        _logger.LogInformation("MangaRepository initialized for database {Database}", options.Value.DatabaseName);
    }

    // Create
    public async Task<Book.Manga> CreateAsync(Book.Manga manga, CancellationToken cancellationToken = default)
    {
        _logger.LogInformation("Creating manga {Title} v{Volume}", manga.Title, manga.Volume);
        await _collection.InsertOneAsync(manga, cancellationToken: cancellationToken);
        _logger.LogInformation("Created manga with id {Id}", manga.Id);
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
    {
        var manga = await _collection.Find(m => m.Id == id).FirstOrDefaultAsync(cancellationToken);
        if (manga is null) _logger.LogWarning("Manga {Id} not found", id);
        return manga;
    }

    // Read - Get by title
    public async Task<Book.Manga> GetByTitleAsync(string title, CancellationToken cancellationToken = default)
    {
        var manga = await _collection.Find(m => m.Title == title && m.CollectionType == Collection.Manga)
            .FirstOrDefaultAsync(cancellationToken);
        if (manga is null) _logger.LogWarning("Manga {Title} not found", title);
        return manga;
    }

    // Update
    public async Task<bool> UpdateAsync(string id, Book.Manga updatedManga,
        CancellationToken cancellationToken = default)
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
            .Set(m => m.PublishDate, updatedManga.PublishDate)
            .Set(m => m.UpdatedAt, DateTime.UtcNow);

        _logger.LogInformation("Updating manga {Id} to title {Title} v{Volume}", id, updatedManga.Title,
            updatedManga.Volume);
        var result = await _collection.UpdateOneAsync(
            m => m.Id == id,
            update,
            cancellationToken: cancellationToken);
        var ok = result.IsAcknowledged && result.ModifiedCount > 0;
        if (!ok) _logger.LogWarning("No manga updated for {Id}", id);
        return ok;
    }

    // Delete
    public async Task<bool> DeleteAsync(string id, CancellationToken cancellationToken = default)
    {
        _logger.LogInformation("Deleting manga {Id}", id);
        var result = await _collection.DeleteOneAsync(m => m.Id == id, cancellationToken);
        var ok = result.IsAcknowledged && result.DeletedCount > 0;
        if (!ok) _logger.LogWarning("No manga deleted for {Id}", id);
        return ok;
    }

    // Search by title (prefix, case-insensitive via collation). Uses index on Title.
    public async Task<List<Book.Manga>> SearchByTitleAsync(string titleQuery, CancellationToken cancellationToken = default)
    {
        if (string.IsNullOrWhiteSpace(titleQuery))
        {
            return await GetAllAsync(cancellationToken);
        }

        // Prefix boundaries (e.g., "Cla" => GTE "Cla" AND LT "Cla\uffff")
        var prefix = titleQuery.Trim();
        var upperBound = prefix + "\uffff";

        var builder = Builders<Book.Manga>.Filter;
        var filter = builder.And(
            builder.Gte(m => m.Title, prefix),
            builder.Lt(m => m.Title, upperBound),
            builder.Eq(m => m.CollectionType, Collection.Manga)
        );

        var options = new FindOptions<Book.Manga>
        {
            Collation = new Collation("simple", strength: CollationStrength.Secondary) // case-insensitive
        };

        using var cursor = await _collection.FindAsync(filter, options, cancellationToken);
        return await cursor.ToListAsync(cancellationToken);
    }
}