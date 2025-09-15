using MyLittleLibrary.Domain;
using MyLittleLibrary.Application;
using Microsoft.Extensions.Logging;

namespace MyLittleLibrary.Infrastructure;

public class LightNovelService : ILightNovelService
{
    private readonly LightNovelRepository _repository;
    private readonly ILogger<LightNovelService> _logger;

    public LightNovelService(LightNovelRepository repository, ILogger<LightNovelService> logger)
    {
        _repository = repository;
        _logger = logger;
    }

    public Task<List<Book.LightNovel>> GetAllAsync(CancellationToken cancellationToken = default) => _repository.GetAllAsync(cancellationToken);

    public Task<List<Book.LightNovel>> GetAllByTitleAsync(string title, CancellationToken cancellationToken = default) => _repository.GetAllByTitleAsync(title, cancellationToken);

    public Task<Book.LightNovel> GetByIdAsync(string id, CancellationToken cancellationToken = default) => _repository.GetByIdAsync(id, cancellationToken);

    public Task<Book.LightNovel> GetByTitleAsync(string title, CancellationToken cancellationToken = default) => _repository.GetByTitleAsync(title, cancellationToken);

    public Task<Book.LightNovel> CreateAsync(Book.LightNovel lightNovel, CancellationToken cancellationToken = default) => _repository.CreateAsync(lightNovel, cancellationToken);

    public Task<bool> UpdateAsync(string id, Book.LightNovel updatedLightNovel, CancellationToken cancellationToken = default) => _repository.UpdateAsync(id, updatedLightNovel, cancellationToken);

    public Task<bool> DeleteAsync(string id, CancellationToken cancellationToken = default) => _repository.DeleteAsync(id, cancellationToken);

    public Task<List<Book.LightNovel>> SearchByTitleAsync(string titleQuery, CancellationToken cancellationToken = default) => _repository.SearchByTitleAsync(titleQuery, cancellationToken);
}