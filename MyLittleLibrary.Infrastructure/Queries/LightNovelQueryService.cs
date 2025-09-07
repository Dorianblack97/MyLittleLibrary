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

    public Task<List<Book.LightNovel>> GetAllAsync(CancellationToken cancellationToken = default) => _repository.GetAllAsync(cancellationToken);

    public Task<List<Book.LightNovel>> GetAllByTitleAsync(string title, CancellationToken cancellationToken = default) => _repository.GetAllByTitleAsync(title, cancellationToken);

    public Task<Book.LightNovel> GetByIdAsync(string id, CancellationToken cancellationToken = default) => _repository.GetByIdAsync(id, cancellationToken);

    public Task<Book.LightNovel> GetByTitleAsync(string title, CancellationToken cancellationToken = default) => _repository.GetByTitleAsync(title, cancellationToken);

    public Task<List<Book.LightNovel>> SearchByTitleAsync(string titleQuery, CancellationToken cancellationToken = default) => _repository.SearchByTitleAsync(titleQuery, cancellationToken);
}