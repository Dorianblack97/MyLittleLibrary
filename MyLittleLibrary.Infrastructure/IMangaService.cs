using MyLittleLibrary.Domain;

namespace MyLittleLibrary.Infrastructure;

public interface IMangaService
{
    Task<List<Book.Manga>> GetAllAsync();
    Task<List<Book.Manga>> GetAllByTitleAsync(string title);
    Task<Book.Manga> GetByIdAsync(string id);
    Task<Book.Manga> GetByTitleAsync(string title);
    Task<Book.Manga> CreateAsync(Book.Manga manga);
    Task<bool> UpdateAsync(string id, Book.Manga updatedManga);
    Task<bool> DeleteAsync(string id);
    Task<List<Book.Manga>> SearchByTitleAsync(string titleQuery);
}