using MyLittleLibrary.Domain;

namespace MyLittleLibrary.Application.Queries;

public interface IMangaQueryService
{
    Task<List<Book.Manga>> GetAllAsync(CancellationToken cancellationToken = default);
    Task<List<Book.Manga>> GetAllByTitleAsync(string title, CancellationToken cancellationToken = default);
    Task<Book.Manga> GetByIdAsync(string id, CancellationToken cancellationToken = default);
    Task<Book.Manga> GetByTitleAsync(string title, CancellationToken cancellationToken = default);
    Task<List<Book.Manga>> SearchByTitleAsync(string titleQuery, CancellationToken cancellationToken = default);
}