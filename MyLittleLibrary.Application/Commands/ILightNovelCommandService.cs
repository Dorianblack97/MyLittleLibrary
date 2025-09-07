using MyLittleLibrary.Domain;

namespace MyLittleLibrary.Application.Commands;

public interface ILightNovelCommandService
{
    Task<Book.LightNovel> CreateAsync(Book.LightNovel lightNovel, CancellationToken cancellationToken = default);
    Task<bool> UpdateAsync(string id, Book.LightNovel updatedLightNovel, CancellationToken cancellationToken = default);
    Task<bool> DeleteAsync(string id, CancellationToken cancellationToken = default);
}