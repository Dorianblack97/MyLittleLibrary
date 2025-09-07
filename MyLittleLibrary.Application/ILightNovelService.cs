using MyLittleLibrary.Domain;

namespace MyLittleLibrary.Application;

public interface ILightNovelService
{
    Task<List<Book.LightNovel>> GetAllAsync(CancellationToken cancellationToken = default);
    Task<List<Book.LightNovel>> GetAllByTitleAsync(string title, CancellationToken cancellationToken = default);
    Task<Book.LightNovel> GetByIdAsync(string id, CancellationToken cancellationToken = default);
    Task<Book.LightNovel> GetByTitleAsync(string title, CancellationToken cancellationToken = default);
    Task<Book.LightNovel> CreateAsync(Book.LightNovel lightNovel, CancellationToken cancellationToken = default);
    Task<bool> UpdateAsync(string id, Book.LightNovel updatedLightNovel, CancellationToken cancellationToken = default);
    Task<bool> DeleteAsync(string id, CancellationToken cancellationToken = default);
    Task<List<Book.LightNovel>> SearchByTitleAsync(string titleQuery, CancellationToken cancellationToken = default);
}