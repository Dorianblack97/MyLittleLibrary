using MyLittleLibrary.Domain;

namespace MyLittleLibrary.Application.Commands;

public interface ILightNovelCommandService
{
    Task<Book.LightNovel> CreateAsync(Book.LightNovel lightNovel);
    Task<bool> UpdateAsync(string id, Book.LightNovel updatedLightNovel);
    Task<bool> DeleteAsync(string id);
}