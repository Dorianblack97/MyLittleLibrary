
using Microsoft.Extensions.Options;
using MongoDB.Bson;
using MongoDB.Driver;
using MyLittleLibrary.Domain;
using MyLittleLibrary.Infrastructure.Constants;
using MyLittleLibrary.Infrastructure.Options;

using Microsoft.Extensions.Logging;

namespace MyLittleLibrary.Infrastructure;

public class FilmRepository
{
    private readonly IMongoCollection<Video.Film> _collection;
    private readonly ILogger<FilmRepository> _logger;

    public FilmRepository(IOptions<MongoOptions> options, ILogger<FilmRepository> logger)
    {
        _logger = logger;
        var client = new MongoClient(options.Value.ConnectionString);
        var database = client.GetDatabase(options.Value.DatabaseName);
        _collection = database.GetCollection<Video.Film>(MongoDbContracts.MongoCollection);
        _logger.LogInformation("FilmRepository initialized for {Db}", options.Value.DatabaseName);
    }

    // Create
    public async Task<Video.Film> CreateAsync(Video.Film film, CancellationToken cancellationToken = default)
    {
        _logger.LogInformation("Creating film {Title}", film.Title);
        await _collection.InsertOneAsync(film, cancellationToken: cancellationToken);
        _logger.LogInformation("Created film {Id}", film.Id);
        return film;
    }

    // Read - Get all
    public async Task<List<Video.Film>> GetAllAsync(CancellationToken cancellationToken = default) 
        => await _collection.Find(f => f.CollectionType == Collection.Film).ToListAsync(cancellationToken);

    // Read - Get all by title
    public async Task<List<Video.Film>> GetAllByTitleAsync(string title, CancellationToken cancellationToken = default) 
        => await _collection.Find(f => f.Title == title && f.CollectionType == Collection.Film).ToListAsync(cancellationToken);

    // Read - Get by ID
    public async Task<Video.Film> GetByIdAsync(string id, CancellationToken cancellationToken = default)
    {
        var film = await _collection.Find(f => f.Id == id).FirstOrDefaultAsync(cancellationToken);
        if (film is null) _logger.LogWarning("Film {Id} not found", id);
        return film;       
    }

    // Read - Get by title
    public async Task<Video.Film> GetByTitleAsync(string title, CancellationToken cancellationToken = default)
    {
        var film = await _collection.Find(f => f.Title == title && f.CollectionType == Collection.Film)
            .FirstOrDefaultAsync(cancellationToken);
        if (film is null) _logger.LogWarning("Film {Title} not found", title);
        return film;       
    }

    // Read - Get by director
    public async Task<List<Video.Film>> GetByDirectorAsync(string director, CancellationToken cancellationToken = default) 
        => await _collection.Find(f => f.Director == director && f.CollectionType == Collection.Film).ToListAsync(cancellationToken);

    // Update
    public async Task<bool> UpdateAsync(string id, Video.Film updatedFilm, CancellationToken cancellationToken = default)
    {
        var update = Builders<Video.Film>.Update
            .Set(f => f.Title, updatedFilm.Title)
            .Set(f => f.TitleSlug, updatedFilm.TitleSlug)
            .Set(f => f.Director, updatedFilm.Director)
            .Set(f => f.Format, updatedFilm.Format)
            .Set(f => f.IsWatched, updatedFilm.IsWatched)
            .Set(f => f.ReleaseDate, updatedFilm.ReleaseDate)
            .Set(f => f.ImagePath, updatedFilm.ImagePath);

        _logger.LogInformation("Updating film {Id} to title {Title}", id, updatedFilm.Title);
        var result = await _collection.UpdateOneAsync(
            f => f.Id == id,
            update,
            cancellationToken: cancellationToken);

        var ok = result.IsAcknowledged && result.ModifiedCount > 0;
        if (!ok) _logger.LogWarning("No film updated for {Id}", id);
        return ok;       
    }

    // Delete
    public async Task<bool> DeleteAsync(string id, CancellationToken cancellationToken = default)
    {
        _logger.LogInformation("Deleting film {Id}", id);      
        var result = await _collection.DeleteOneAsync(f => f.Id == id, cancellationToken);
        var ok = result.IsAcknowledged && result.DeletedCount > 0;
        if (!ok) _logger.LogWarning("No film deleted for {Id}", id);
        return ok;       
    }

    // Search by title (prefix, case-insensitive via collation). Uses index on Title.
    public async Task<List<Video.Film>> SearchByTitleAsync(string titleQuery, CancellationToken cancellationToken = default)
    {
        if (string.IsNullOrWhiteSpace(titleQuery))
        {
            return await GetAllAsync(cancellationToken);
        }

        var prefix = titleQuery.Trim();
        var upperBound = prefix + "\uffff";

        var builder = Builders<Video.Film>.Filter;
        var filter = builder.And(
            builder.Gte(f => f.Title, prefix),
            builder.Lt(f => f.Title, upperBound),
            builder.Eq(f => f.CollectionType, Collection.Film)
        );

        var options = new FindOptions<Video.Film>
        {
            Collation = new Collation("simple", strength: CollationStrength.Secondary)
        };

        using var cursor = await _collection.FindAsync(filter, options, cancellationToken);
        return await cursor.ToListAsync(cancellationToken);
    }

    // Search by director (partial match)
    public async Task<List<Video.Film>> SearchByDirectorAsync(string directorQuery, CancellationToken cancellationToken = default)
    {
        var filter = Builders<Video.Film>.Filter.And(
            Builders<Video.Film>.Filter.Regex(f => f.Director, new BsonRegularExpression(directorQuery, "i")),
            Builders<Video.Film>.Filter.Eq(f => f.CollectionType, Collection.Film)
        );
        return await _collection.Find(filter).ToListAsync(cancellationToken);
    }

    // Get unwatched films
    public async Task<List<Video.Film>> GetUnwatchedAsync(CancellationToken cancellationToken = default) 
        => await _collection.Find(f => !f.IsWatched && f.CollectionType == Collection.Film).ToListAsync(cancellationToken);

    // Get watched films
    public async Task<List<Video.Film>> GetWatchedAsync(CancellationToken cancellationToken = default) 
        => await _collection.Find(f => f.IsWatched && f.CollectionType == Collection.Film).ToListAsync(cancellationToken);

    // Get films by format
    public async Task<List<Video.Film>> GetByFormatAsync(VideoFormat format, CancellationToken cancellationToken = default) 
        => await _collection.Find(f => f.Format == format && f.CollectionType == Collection.Film).ToListAsync(cancellationToken);
}