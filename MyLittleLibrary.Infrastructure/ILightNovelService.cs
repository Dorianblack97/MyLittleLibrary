using MyLittleLibrary.Domain;

namespace MyLittleLibrary.Infrastructure;

public interface ILightNovelService
{
    Task<List<Book.LightNovel>> GetAllAsync();
    Task<List<Book.LightNovel>> GetAllByTitleAsync(string title);
    Task<Book.LightNovel> GetByIdAsync(string id);
    Task<Book.LightNovel> GetByTitleAsync(string title);
    Task<Book.LightNovel> CreateAsync(Book.LightNovel lightNovel);
    Task<bool> UpdateAsync(string id, Book.LightNovel updatedLightNovel);
    Task<bool> DeleteAsync(string id);
    Task<List<Book.LightNovel>> SearchByTitleAsync(string titleQuery);
}