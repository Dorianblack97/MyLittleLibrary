using MongoDB.Bson;
using MongoDB.Driver;
using MyLittleLibrary.Infrastructure.Constants;

namespace MyLittleLibrary.Infrastructure;

public static class MongoIndexInitializer
{
    public static async Task EnsureIndexesAsync(string connectionString, string databaseName, CancellationToken cancellationToken = default)
    {
        var client = new MongoClient(connectionString);
        var db = client.GetDatabase(databaseName);

        // The shared collection for all domain types
        var collection = db.GetCollection<BsonDocument>(MongoDbContracts.MongoCollection);

        // 1) Title (case-insensitive)
        // Use collation with strength 2 for case-insensitive comparisons.
        // Locale set to "en" works well for English/Italian and ASCII Japanese titles (romanized).
        var titleIndexModel = new CreateIndexModel<BsonDocument>(
            Builders<BsonDocument>.IndexKeys.Ascending("Title"),
            new CreateIndexOptions<BsonDocument>
            {
                Name = "idx_Title_ci",
                Collation = new Collation(locale: "simple", strength: CollationStrength.Secondary)
            });

        // 2) TitleSlug (exact match)
        var slugIndexModel = new CreateIndexModel<BsonDocument>(
            Builders<BsonDocument>.IndexKeys.Ascending("TitleSlug"),
            new CreateIndexOptions<BsonDocument>
            {
                Name = "idx_TitleSlug"
            });

        // 3) CollectionType (exact match)
        var typeIndexModel = new CreateIndexModel<BsonDocument>(
            Builders<BsonDocument>.IndexKeys.Ascending("CollectionType"),
            new CreateIndexOptions<BsonDocument>
            {
                Name = "idx_CollectionType"
            });

        // 4) Compound unique (Title, Volume) only where Volume exists.
        // This prevents conflicts with Film documents that don't have a Volume field.
        var partialFilter = new BsonDocument("Volume", new BsonDocument("$exists", true));
        var titleVolumeUniqueModel = new CreateIndexModel<BsonDocument>(
            Builders<BsonDocument>.IndexKeys.Ascending("Title").Ascending("Volume"),
            new CreateIndexOptions<BsonDocument>
            {
                Name = "ux_Title_Volume",
                Unique = true,
                Sparse = true, // exclude documents without Volume (e.g., films)
                Collation = new Collation(locale: "simple", strength: CollationStrength.Secondary)
            });

        await collection.Indexes.CreateManyAsync(new[]
        {
            titleIndexModel,
            slugIndexModel,
            typeIndexModel,
            titleVolumeUniqueModel
        }, cancellationToken);
    }
}
