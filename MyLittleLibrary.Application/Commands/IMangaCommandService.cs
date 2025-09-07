using MyLittleLibrary.Domain;

namespace MyLittleLibrary.Application.Commands;

public interface IMangaCommandService
{
    Task<Book.Manga> CreateAsync(Book.Manga manga, CancellationToken cancellationToken = default);
    Task<bool> UpdateAsync(string id, Book.Manga updatedManga, CancellationToken cancellationToken = default);
    Task<bool> DeleteAsync(string id, CancellationToken cancellationToken = default);
}