using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace MyLittleLibrary.Domain;

public record Book
{
    public Book(string title, string titleSlug, string author, string? imagePath, bool isDigital,
        bool isRead, DateTime? publishDate, string id = null)
    {
        Title = title;
        TitleSlug = titleSlug;
        Author = author;
        ImagePath = imagePath;
        IsDigital = isDigital;
        IsRead = isRead;
        PublishDate = publishDate;
    }

    [BsonId]
    [BsonRepresentation(BsonType.ObjectId)]
    public string Id { get; init; } = ObjectId.GenerateNewId().ToString();
    public string Title { get; init; }
    public string TitleSlug { get; init; }
    public string Author { get; init; }
    public string? ImagePath { get; init; }
    public bool IsDigital { get; init; }
    public bool IsRead { get; init; }
    public DateTime? PublishDate { get; init; }
}