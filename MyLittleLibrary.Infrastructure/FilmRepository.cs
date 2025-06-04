
using Microsoft.Extensions.Options;
using MongoDB.Bson;
using MongoDB.Driver;
using MyLittleLibrary.Domain;
using MyLittleLibrary.Infrastructure.Constants;
using MyLittleLibrary.Infrastructure.Options;

namespace MyLittleLibrary.Infrastructure;

public class FilmRepository
{
    private readonly IMongoCollection<Video.Film> _collection;

    public FilmRepository(IOptions<MongoOptions> options)
    {
        var client = new MongoClient(options.Value.ConnectionString);
        var database = client.GetDatabase(options.Value.DatabaseName);
        _collection = database.GetCollection<Video.Film>(MongoDbContracts.MongoCollection);
    }

    // Create
    public async Task<Video.Film> CreateAsync(Video.Film film)
    {
        await _collection.InsertOneAsync(film);
        return film;
    }

    // Read - Get all
    public async Task<List<Video.Film>> GetAllAsync() 
        => await _collection.Find(f => f.CollectionType == Collection.Film).ToListAsync();

    // Read - Get all by title
    public async Task<List<Video.Film>> GetAllByTitleAsync(string title) 
        => await _collection.Find(f => f.Title == title && f.CollectionType == Collection.Film).ToListAsync();

    // Read - Get by ID
    public async Task<Video.Film> GetByIdAsync(string id) 
        => await _collection.Find(f => f.Id == id).FirstOrDefaultAsync();

    // Read - Get by title
    public async Task<Video.Film> GetByTitleAsync(string title) 
        => await _collection.Find(f => f.Title == title && f.CollectionType == Collection.Film).FirstOrDefaultAsync();

    // Read - Get by director
    public async Task<List<Video.Film>> GetByDirectorAsync(string director) 
        => await _collection.Find(f => f.Director == director && f.CollectionType == Collection.Film).ToListAsync();

    // Update
    public async Task<bool> UpdateAsync(string id, Video.Film updatedFilm)
    {
        var update = Builders<Video.Film>.Update
            .Set(f => f.Title, updatedFilm.Title)
            .Set(f => f.TitleSlug, updatedFilm.TitleSlug)
            .Set(f => f.Director, updatedFilm.Director)
            .Set(f => f.Format, updatedFilm.Format)
            .Set(f => f.IsWatched, updatedFilm.IsWatched)
            .Set(f => f.ReleaseDate, updatedFilm.ReleaseDate)
            .Set(f => f.ImagePath, updatedFilm.ImagePath);

        var result = await _collection.UpdateOneAsync(
            f => f.Id == id,
            update);

        return result.IsAcknowledged && result.ModifiedCount > 0;
    }

    // Delete
    public async Task<bool> DeleteAsync(string id)
    {
        var result = await _collection.DeleteOneAsync(f => f.Id == id);
        return result.IsAcknowledged && result.DeletedCount > 0;
    }

    // Search by title (partial match)
    public async Task<List<Video.Film>> SearchByTitleAsync(string titleQuery)
    {
        var filter = Builders<Video.Film>.Filter.And(
            Builders<Video.Film>.Filter.Regex(f => f.Title, new BsonRegularExpression(titleQuery, "i")),
            Builders<Video.Film>.Filter.Eq(f => f.CollectionType, Collection.Film)
        );
        return await _collection.Find(filter).ToListAsync();
    }

    // Search by director (partial match)
    public async Task<List<Video.Film>> SearchByDirectorAsync(string directorQuery)
    {
        var filter = Builders<Video.Film>.Filter.And(
            Builders<Video.Film>.Filter.Regex(f => f.Director, new BsonRegularExpression(directorQuery, "i")),
            Builders<Video.Film>.Filter.Eq(f => f.CollectionType, Collection.Film)
        );
        return await _collection.Find(filter).ToListAsync();
    }

    // Get unwatched films
    public async Task<List<Video.Film>> GetUnwatchedAsync() 
        => await _collection.Find(f => !f.IsWatched && f.CollectionType == Collection.Film).ToListAsync();

    // Get watched films
    public async Task<List<Video.Film>> GetWatchedAsync() 
        => await _collection.Find(f => f.IsWatched && f.CollectionType == Collection.Film).ToListAsync();

    // Get films by format
    public async Task<List<Video.Film>> GetByFormatAsync(VideoFormat format) 
        => await _collection.Find(f => f.Format == format && f.CollectionType == Collection.Film).ToListAsync();
}