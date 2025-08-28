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

    public Task<Book.LightNovel> CreateAsync(Book.LightNovel lightNovel) => _repository.CreateAsync(lightNovel);

    public Task<bool> UpdateAsync(string id, Book.LightNovel updatedLightNovel) => _repository.UpdateAsync(id, updatedLightNovel);

    public Task<bool> DeleteAsync(string id) => _repository.DeleteAsync(id);
}