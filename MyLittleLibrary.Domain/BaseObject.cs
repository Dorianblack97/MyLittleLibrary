using System.Text.RegularExpressions;
using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace MyLittleLibrary.Domain;

public record BaseObject
{
    private static readonly Regex SlugRegex = new("^[A-Za-z0-9]+(?:-[A-Za-z0-9]+)*$", RegexOptions.Compiled);

    public BaseObject(string title, string titleSlug, string? imagePath, Collection collectionType, DateTime timestamp, string id = null, DateTime? updatedAt = null)
    {
        if (string.IsNullOrWhiteSpace(title))
            throw new ArgumentException("Title must be provided and not be empty or whitespace.", nameof(title));

        if (string.IsNullOrWhiteSpace(titleSlug))
            throw new ArgumentException("TitleSlug must be provided and not be empty.", nameof(titleSlug));

        if (!IsValidSlug(titleSlug))
            throw new ArgumentException("TitleSlug is invalid. Allowed: letters/numbers with optional hyphen separators (e.g., 'my-title', 'MyTitle').", nameof(titleSlug));

        Id = id;
        Title = title;
        TitleSlug = titleSlug;
        ImagePath = imagePath;
        CollectionType = collectionType;
        CreateAt = NormalizeToUtc(timestamp);
        UpdatedAt = updatedAt is null ? DateTime.UtcNow : NormalizeToUtc(updatedAt.Value);
    }

    private static bool IsValidSlug(string slug)
        => SlugRegex.IsMatch(slug);

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