using MyLittleLibrary.Domain;

namespace MyLittleLibrary.Application.Commands;

public interface IMangaCommandService
{
    Task<Book.Manga> CreateAsync(Book.Manga manga);
    Task<bool> UpdateAsync(string id, Book.Manga updatedManga);
    Task<bool> DeleteAsync(string id);
}