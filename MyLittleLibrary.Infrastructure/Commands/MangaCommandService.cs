using MyLittleLibrary.Domain;
using MyLittleLibrary.Application.Commands;

namespace MyLittleLibrary.Infrastructure.Commands;

public class MangaCommandService : IMangaCommandService
{
    private readonly MangaRepository _repository;

    public MangaCommandService(MangaRepository repository)
    {
        _repository = repository;
    }

    public Task<Book.Manga> CreateAsync(Book.Manga manga) => _repository.CreateAsync(manga);

    public Task<bool> UpdateAsync(string id, Book.Manga updatedManga) => _repository.UpdateAsync(id, updatedManga);

    public Task<bool> DeleteAsync(string id) => _repository.DeleteAsync(id);
}