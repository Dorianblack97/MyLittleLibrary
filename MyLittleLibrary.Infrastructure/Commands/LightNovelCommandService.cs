using MyLittleLibrary.Domain;
using MyLittleLibrary.Application.Commands;

namespace MyLittleLibrary.Infrastructure.Commands;

public class LightNovelCommandService : ILightNovelCommandService
{
    private readonly LightNovelRepository _repository;

    public LightNovelCommandService(LightNovelRepository repository)
    {
        _repository = repository;
    }

    public Task<Book.LightNovel> CreateAsync(Book.LightNovel lightNovel, CancellationToken cancellationToken = default) => _repository.CreateAsync(lightNovel, cancellationToken);

    public Task<bool> UpdateAsync(string id, Book.LightNovel updatedLightNovel, CancellationToken cancellationToken = default) => _repository.UpdateAsync(id, updatedLightNovel, cancellationToken);

    public Task<bool> DeleteAsync(string id, CancellationToken cancellationToken = default) => _repository.DeleteAsync(id, cancellationToken);
}