namespace MyLittleLibrary.Domain;

public record Video : BaseObject
{
    private Video(string title, string titleSlug, string? imagePath, Collection collection, string id = null) : 
        base(title, titleSlug, imagePath, collection, DateTime.UtcNow)
    { }
    
    public sealed record Film : Video
    {
        public Film(string title, string titleSlug, string director, string? imagePath, 
            VideoFormat format, bool isWatched, DateTime? releaseDate, string id = null) 
            : base(title, titleSlug, imagePath, Collection.Film, id)
        {
            Director = director;
            Format = format;
            IsWatched = isWatched;
            ReleaseDate = releaseDate;
        }

        public string Director { get; init; }
        public VideoFormat Format { get; init; }
        public bool IsWatched { get; init; }
        public DateTime? ReleaseDate { get; init; }
    }
}