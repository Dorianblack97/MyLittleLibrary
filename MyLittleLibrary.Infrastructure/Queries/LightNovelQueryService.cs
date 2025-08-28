using MyLittleLibrary.Domain;
using MyLittleLibrary.Application.Queries;

namespace MyLittleLibrary.Infrastructure.Queries;

public class LightNovelQueryService : ILightNovelQueryService
{
    private readonly LightNovelRepository _repository;

    public LightNovelQueryService(LightNovelRepository repository)
    {
        _repository = repository;
    }

    public Task<List<Book.LightNovel>> GetAllAsync() => _repository.GetAllAsync();

    public Task<List<Book.LightNovel>> GetAllByTitleAsync(string title) => _repository.GetAllByTitleAsync(title);

    public Task<Book.LightNovel> GetByIdAsync(string id) => _repository.GetByIdAsync(id);

    public Task<Book.LightNovel> GetByTitleAsync(string title) => _repository.GetByTitleAsync(title);

    public Task<List<Book.LightNovel>> SearchByTitleAsync(string titleQuery) => _repository.SearchByTitleAsync(titleQuery);
}