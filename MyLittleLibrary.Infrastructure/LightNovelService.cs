using MyLittleLibrary.Domain;
using MyLittleLibrary.Application;

namespace MyLittleLibrary.Infrastructure;

public class LightNovelService : ILightNovelService
{
    private readonly LightNovelRepository _repository;

    public LightNovelService(LightNovelRepository repository)
    {
        _repository = repository;
    }

    public Task<List<Book.LightNovel>> GetAllAsync() => _repository.GetAllAsync();

    public Task<List<Book.LightNovel>> GetAllByTitleAsync(string title) => _repository.GetAllByTitleAsync(title);

    public Task<Book.LightNovel> GetByIdAsync(string id) => _repository.GetByIdAsync(id);

    public Task<Book.LightNovel> GetByTitleAsync(string title) => _repository.GetByTitleAsync(title);

    public Task<Book.LightNovel> CreateAsync(Book.LightNovel lightNovel) => _repository.CreateAsync(lightNovel);

    public Task<bool> UpdateAsync(string id, Book.LightNovel updatedLightNovel) => _repository.UpdateAsync(id, updatedLightNovel);

    public Task<bool> DeleteAsync(string id) => _repository.DeleteAsync(id);

    public Task<List<Book.LightNovel>> SearchByTitleAsync(string titleQuery) => _repository.SearchByTitleAsync(titleQuery);
}