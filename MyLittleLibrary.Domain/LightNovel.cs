namespace MyLittleLibrary.Domain;

public record LightNovel : Book
{
    public LightNovel(string title, string titleSlug, string author, string illustrator, int volume, string? imagePath, 
        bool isDigital, bool isRead, DateTime? publishDate, string id = null) 
        : base(title, titleSlug, author, imagePath, isDigital, isRead, publishDate, id)
    {
        Title = title;
        TitleSlug = titleSlug;
        Author = author;
        Illustrator = illustrator;
        Volume = volume;
        ImagePath = imagePath;
        IsDigital = isDigital;
        IsRead = isRead;
        PublishDate = publishDate;
    }

    public int Volume { get; init; }
    public string Illustrator { get; init; }
}