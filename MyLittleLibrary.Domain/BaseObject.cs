using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace MyLittleLibrary.Domain;

public record BaseObject
{
    public BaseObject(string title, string titleSlug, string? imagePath, Collection collectionType, DateTime timestamp, string id = null, DateTime? updatedAt = null)
    {
        Id = id;
        Title = title;
        TitleSlug = titleSlug;
        ImagePath = imagePath;
        CollectionType = collectionType;
        CreateAt = NormalizeToUtc(timestamp);
        UpdatedAt = updatedAt is null ? DateTime.UtcNow : NormalizeToUtc(updatedAt.Value);
    }

    private static DateTime NormalizeToUtc(DateTime dt)
        => dt.Kind switch
        {
            DateTimeKind.Utc => dt,
            DateTimeKind.Local => dt.ToUniversalTime(),
            _ => DateTime.SpecifyKind(dt, DateTimeKind.Utc)
        };

    [BsonId]
    [BsonRepresentation(BsonType.ObjectId)]
    public string Id { get; init; } = ObjectId.GenerateNewId().ToString();
    public string Title { get; init; }
    public string TitleSlug { get; init; }
    public string? ImagePath { get; init; }
    public Collection CollectionType { get; init; }
    public DateTime CreateAt { get; init; }
    public DateTime? UpdatedAt { get; init; }
}