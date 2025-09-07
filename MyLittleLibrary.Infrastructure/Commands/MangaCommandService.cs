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

    public Task<Book.Manga> CreateAsync(Book.Manga manga, CancellationToken cancellationToken = default) => _repository.CreateAsync(manga, cancellationToken);

    public Task<bool> UpdateAsync(string id, Book.Manga updatedManga, CancellationToken cancellationToken = default) => _repository.UpdateAsync(id, updatedManga, cancellationToken);

    public Task<bool> DeleteAsync(string id, CancellationToken cancellationToken = default) => _repository.DeleteAsync(id, cancellationToken);
}