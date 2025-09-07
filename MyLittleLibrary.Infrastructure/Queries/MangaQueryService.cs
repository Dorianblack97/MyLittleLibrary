using MyLittleLibrary.Domain;
using MyLittleLibrary.Application.Queries;

namespace MyLittleLibrary.Infrastructure.Queries;

public class MangaQueryService : IMangaQueryService
{
    private readonly MangaRepository _repository;

    public MangaQueryService(MangaRepository repository)
    {
        _repository = repository;
    }

    public Task<List<Book.Manga>> GetAllAsync(CancellationToken cancellationToken = default) => _repository.GetAllAsync(cancellationToken);

    public Task<List<Book.Manga>> GetAllByTitleAsync(string title, CancellationToken cancellationToken = default) => _repository.GetAllByTitleAsync(title, cancellationToken);

    public Task<Book.Manga> GetByIdAsync(string id, CancellationToken cancellationToken = default) => _repository.GetByIdAsync(id, cancellationToken);

    public Task<Book.Manga> GetByTitleAsync(string title, CancellationToken cancellationToken = default) => _repository.GetByTitleAsync(title, cancellationToken);

    public Task<List<Book.Manga>> SearchByTitleAsync(string titleQuery, CancellationToken cancellationToken = default) => _repository.SearchByTitleAsync(titleQuery, cancellationToken);
}