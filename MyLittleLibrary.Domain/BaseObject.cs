using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace MyLittleLibrary.Domain;

public record BaseObject
{
    public BaseObject(string title, string titleSlug, string? imagePath, Collection collectionType, DateTime timestamp, string id = null)
    {
        Id = id;
        Title = title;
        TitleSlug = titleSlug;
        ImagePath = imagePath;
        CollectionType = collectionType;
        Timestamp = timestamp;
    }

    [BsonId]
    [BsonRepresentation(BsonType.ObjectId)]
    public string Id { get; init; } = ObjectId.GenerateNewId().ToString();
    public string Title { get; init; }
    public string TitleSlug { get; init; }
    public string? ImagePath { get; init; }
    public Collection CollectionType { get; init; }
    public DateTime Timestamp { get; init; }
}