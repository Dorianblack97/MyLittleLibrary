using Microsoft.Extensions.Options;
using MongoDB.Driver;
using MyLittleLibrary.Domain;
using MyLittleLibrary.Infrastructure.Options;
using MongoDB.Bson;

namespace MyLittleLibrary.Infrastructure;

public class BaseObjectRepository
{
    private readonly IMongoCollection<BsonDocument> _collection;

    public BaseObjectRepository(IOptions<MongoOptions> options)
    {
        var client = new MongoClient(options.Value.ConnectionString);
        var database = client.GetDatabase(options.Value.DatabaseName);
        _collection = database.GetCollection<BsonDocument>(Contracts.MongoCollection);
    }

    // Get 8 most recent objects with only BaseObject properties
    public async Task<List<BaseObject>> GetMostRecentAsync(int count = 8)
    {
        
        var projection = Builders<BsonDocument>.Projection
            .Include("_id")
            .Include("Title")
            .Include("TitleSlug")
            .Include("ImagePath")
            .Include("CollectionType")
            .Include("Timestamp");

        var documents = await _collection
            .Find(new BsonDocument())
            .Project(projection)
            .Sort(Builders<BsonDocument>.Sort.Descending("Timestamp"))
            .Limit(count)
            .ToListAsync();

        return documents.Select(doc => new BaseObject(
            title: doc.GetValue("Title", "").AsString,
            titleSlug: doc.GetValue("TitleSlug", "").AsString,
            imagePath: doc.GetValue("ImagePath", BsonNull.Value).IsBsonNull ? null : doc["ImagePath"].AsString,
            collectionType: (Collection)doc.GetValue("CollectionType", 0).AsInt32,
            timestamp: doc.GetValue("Timestamp", DateTime.MinValue).ToUniversalTime(),
            id: doc["_id"].AsObjectId.ToString()
        )).ToList();
    }

    // Get 8 most recent objects by collection type with only BaseObject properties
    public async Task<List<BaseObject>> GetMostRecentByTypeAsync(Collection collectionType, int count = 8)
    {
        var projection = Builders<BsonDocument>.Projection
            .Include("_id")
            .Include("Title")
            .Include("TitleSlug")
            .Include("ImagePath")
            .Include("CollectionType")
            .Include("Timestamp");

        var filter = Builders<BsonDocument>.Filter.Eq("CollectionType", (int)collectionType);

        var documents = await _collection
            .Find(filter)
            .Project(projection)
            .Sort(Builders<BsonDocument>.Sort.Descending("Timestamp"))
            .Limit(count)
            .ToListAsync();

        return documents.Select(doc => new BaseObject(
            title: doc.GetValue("Title", "").AsString,
            titleSlug: doc.GetValue("TitleSlug", "").AsString,
            imagePath: doc.GetValue("ImagePath", BsonNull.Value).IsBsonNull ? null : doc["ImagePath"].AsString,
            collectionType: (Collection)doc.GetValue("CollectionType", 0).AsInt32,
            timestamp: doc.GetValue("Timestamp", DateTime.MinValue).ToUniversalTime(),
            id: doc["_id"].AsObjectId.ToString()
        )).ToList();
    }

    // Get all with only BaseObject properties
    public async Task<List<BaseObject>> GetAllAsync()
    {
        var projection = Builders<BsonDocument>.Projection
            .Include("_id")
            .Include("Title")
            .Include("TitleSlug")
            .Include("ImagePath")
            .Include("CollectionType")
            .Include("Timestamp");

        var documents = await _collection
            .Find(new BsonDocument())
            .Project(projection)
            .ToListAsync();

        return documents.Select(doc => new BaseObject(
            title: doc.GetValue("Title", "").AsString,
            titleSlug: doc.GetValue("TitleSlug", "").AsString,
            imagePath: doc.GetValue("ImagePath", BsonNull.Value).IsBsonNull ? null : doc["ImagePath"].AsString,
            collectionType: (Collection)doc.GetValue("CollectionType", 0).AsInt32,
            timestamp: doc.GetValue("Timestamp", DateTime.MinValue).ToUniversalTime(),
            id: doc["_id"].AsObjectId.ToString()
        )).ToList();
    }

    // Get by ID with only BaseObject properties
    public async Task<BaseObject> GetByIdAsync(string id)
    {
        var projection = Builders<BsonDocument>.Projection
            .Include("_id")
            .Include("Title")
            .Include("TitleSlug")
            .Include("ImagePath")
            .Include("CollectionType")
            .Include("Timestamp");

        var filter = Builders<BsonDocument>.Filter.Eq("_id", ObjectId.Parse(id));
        
        var document = await _collection
            .Find(filter)
            .Project(projection)
            .FirstOrDefaultAsync();

        if (document == null) return null;

        return new BaseObject(
            title: document.GetValue("Title", "").AsString,
            titleSlug: document.GetValue("TitleSlug", "").AsString,
            imagePath: document.GetValue("ImagePath", BsonNull.Value).IsBsonNull ? null : document["ImagePath"].AsString,
            collectionType: (Collection)document.GetValue("CollectionType", 0).AsInt32,
            timestamp: document.GetValue("Timestamp", DateTime.MinValue).ToUniversalTime(),
            id: document["_id"].AsObjectId.ToString()
        );
    }
}