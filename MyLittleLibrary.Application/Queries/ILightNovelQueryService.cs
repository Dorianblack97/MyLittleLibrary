using MyLittleLibrary.Domain;

namespace MyLittleLibrary.Application.Queries;

public interface ILightNovelQueryService
{
    Task<List<Book.LightNovel>> GetAllAsync(CancellationToken cancellationToken = default);
    Task<List<Book.LightNovel>> GetAllByTitleAsync(string title, CancellationToken cancellationToken = default);
    Task<Book.LightNovel> GetByIdAsync(string id, CancellationToken cancellationToken = default);
    Task<Book.LightNovel> GetByTitleAsync(string title, CancellationToken cancellationToken = default);
    Task<List<Book.LightNovel>> SearchByTitleAsync(string titleQuery, CancellationToken cancellationToken = default);
}