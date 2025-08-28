using MyLittleLibrary.Domain;

namespace MyLittleLibrary.Application.Queries;

public interface ILightNovelQueryService
{
    Task<List<Book.LightNovel>> GetAllAsync();
    Task<List<Book.LightNovel>> GetAllByTitleAsync(string title);
    Task<Book.LightNovel> GetByIdAsync(string id);
    Task<Book.LightNovel> GetByTitleAsync(string title);
    Task<List<Book.LightNovel>> SearchByTitleAsync(string titleQuery);
}