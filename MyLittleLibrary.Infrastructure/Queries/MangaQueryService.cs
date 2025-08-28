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

    public Task<List<Book.Manga>> GetAllAsync() => _repository.GetAllAsync();

    public Task<List<Book.Manga>> GetAllByTitleAsync(string title) => _repository.GetAllByTitleAsync(title);

    public Task<Book.Manga> GetByIdAsync(string id) => _repository.GetByIdAsync(id);

    public Task<Book.Manga> GetByTitleAsync(string title) => _repository.GetByTitleAsync(title);

    public Task<List<Book.Manga>> SearchByTitleAsync(string titleQuery) => _repository.SearchByTitleAsync(titleQuery);
}