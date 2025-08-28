using MyLittleLibrary.Domain;

namespace MyLittleLibrary.Application.Queries;

public interface IMangaQueryService
{
    Task<List<Book.Manga>> GetAllAsync();
    Task<List<Book.Manga>> GetAllByTitleAsync(string title);
    Task<Book.Manga> GetByIdAsync(string id);
    Task<Book.Manga> GetByTitleAsync(string title);
    Task<List<Book.Manga>> SearchByTitleAsync(string titleQuery);
}