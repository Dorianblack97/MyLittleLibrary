using MyLittleLibrary.Domain;
using MyLittleLibrary.Application;
using Microsoft.Extensions.Logging;

namespace MyLittleLibrary.Infrastructure;

public class MangaService : IMangaService
{
    private readonly MangaRepository _repository;
    private readonly ILogger<MangaService> _logger;

    public MangaService(MangaRepository repository, ILogger<MangaService> logger)
    {
        _repository = repository;
        _logger = logger;
    }

    public Task<List<Book.Manga>> GetAllAsync(CancellationToken cancellationToken = default) => _repository.GetAllAsync(cancellationToken);

    public Task<List<Book.Manga>> GetAllByTitleAsync(string title, CancellationToken cancellationToken = default) => _repository.GetAllByTitleAsync(title, cancellationToken);

    public Task<Book.Manga> GetByIdAsync(string id, CancellationToken cancellationToken = default) => _repository.GetByIdAsync(id, cancellationToken);

    public Task<Book.Manga> GetByTitleAsync(string title, CancellationToken cancellationToken = default) => _repository.GetByTitleAsync(title, cancellationToken);

    public Task<Book.Manga> CreateAsync(Book.Manga manga, CancellationToken cancellationToken = default) => _repository.CreateAsync(manga, cancellationToken);

    public Task<bool> UpdateAsync(string id, Book.Manga updatedManga, CancellationToken cancellationToken = default) => _repository.UpdateAsync(id, updatedManga, cancellationToken);

    public Task<bool> DeleteAsync(string id, CancellationToken cancellationToken = default) => _repository.DeleteAsync(id, cancellationToken);

    public Task<List<Book.Manga>> SearchByTitleAsync(string titleQuery, CancellationToken cancellationToken = default) => _repository.SearchByTitleAsync(titleQuery, cancellationToken);
}