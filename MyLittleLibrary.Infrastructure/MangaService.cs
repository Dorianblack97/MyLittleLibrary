using MyLittleLibrary.Domain;

namespace MyLittleLibrary.Infrastructure;

public class MangaService : IMangaService
{
    private readonly MangaRepository _repository;

    public MangaService(MangaRepository repository)
    {
        _repository = repository;
    }

    public Task<List<Book.Manga>> GetAllAsync() => _repository.GetAllAsync();

    public Task<List<Book.Manga>> GetAllByTitleAsync(string title) => _repository.GetAllByTitleAsync(title);

    public Task<Book.Manga> GetByIdAsync(string id) => _repository.GetByIdAsync(id);

    public Task<Book.Manga> GetByTitleAsync(string title) => _repository.GetByTitleAsync(title);

    public Task<Book.Manga> CreateAsync(Book.Manga manga) => _repository.CreateAsync(manga);

    public Task<bool> UpdateAsync(string id, Book.Manga updatedManga) => _repository.UpdateAsync(id, updatedManga);

    public Task<bool> DeleteAsync(string id) => _repository.DeleteAsync(id);

    public Task<List<Book.Manga>> SearchByTitleAsync(string titleQuery) => _repository.SearchByTitleAsync(titleQuery);
}