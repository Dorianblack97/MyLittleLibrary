using MyLittleLibrary.Domain;

namespace MyLittleLibrary.Application;

public interface IMangaService
{
    Task<List<Book.Manga>> GetAllAsync(CancellationToken cancellationToken = default);
    Task<List<Book.Manga>> GetAllByTitleAsync(string title, CancellationToken cancellationToken = default);
    Task<Book.Manga> GetByIdAsync(string id, CancellationToken cancellationToken = default);
    Task<Book.Manga> GetByTitleAsync(string title, CancellationToken cancellationToken = default);
    Task<Book.Manga> CreateAsync(Book.Manga manga, CancellationToken cancellationToken = default);
    Task<bool> UpdateAsync(string id, Book.Manga updatedManga, CancellationToken cancellationToken = default);
    Task<bool> DeleteAsync(string id, CancellationToken cancellationToken = default);
    Task<List<Book.Manga>> SearchByTitleAsync(string titleQuery, CancellationToken cancellationToken = default);
}