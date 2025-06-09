namespace MyLittleLibrary.Domain;

public record Book : BaseObject
{
    private Book(string title, string titleSlug, string? imagePath, Collection collection, string id = null) : 
        base(title, titleSlug, imagePath, collection, DateTime.UtcNow)
    { }

    public sealed record Standard : Book
    {
        public Standard(string title, string titleSlug, string author, string? imagePath,
            bool isDigital, bool isRead, DateTime? publishDate, string id = null)
            : base(title, titleSlug, imagePath, Collection.Book, id)
        {
            Title = title;
            TitleSlug = titleSlug;
            Author = author;
            ImagePath = imagePath;
            IsDigital = isDigital;
            IsRead = isRead;
            PublishDate = publishDate;
        }

        public string Author { get; init; }
        public bool IsDigital { get; init; }
        public bool IsRead { get; init; }
        public DateTime? PublishDate { get; init; }
    }

    public sealed record Manga : Book
    {
        public Manga(string title, string titleSlug, string author, string illustrator, int volume, 
            string? imagePath, bool isDigital, bool isRead, DateTime? publishDate, string id = null) 
            : base(title, titleSlug, imagePath, Collection.Manga, id)
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
        public string Author { get; init; }
        public int Volume { get; init; }
        public string? Illustrator { get; init; }
        public bool IsDigital { get; init; }
        public bool IsRead { get; init; }
        public DateTime? PublishDate { get; init; }
        
    }
    
    public sealed record LightNovel : Book
    {
        public LightNovel(string title, string titleSlug, string author, string illustrator, string volume, 
            string? imagePath, bool isDigital, bool isRead, DateTime? publishDate, string id = null) 
            : base(title, titleSlug, imagePath, Collection.LightNovel, id)
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

        public string Author { get; init; }
        public string Volume { get; init; }
        public string? Illustrator { get; init; }
        public bool IsDigital { get; init; }
        public bool IsRead { get; init; }
        public DateTime? PublishDate { get; init; }
    }
}