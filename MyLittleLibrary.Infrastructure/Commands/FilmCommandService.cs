using MyLittleLibrary.Domain;
using MyLittleLibrary.Application.Commands;

namespace MyLittleLibrary.Infrastructure.Commands;

public class FilmCommandService : IFilmCommandService
{
    private readonly FilmRepository _repository;

    public FilmCommandService(FilmRepository repository)
    {
        _repository = repository;
    }

    public Task<Video.Film> CreateAsync(Video.Film film, CancellationToken cancellationToken = default) => _repository.CreateAsync(film, cancellationToken);

    public Task<bool> UpdateAsync(string id, Video.Film updatedFilm, CancellationToken cancellationToken = default) => _repository.UpdateAsync(id, updatedFilm, cancellationToken);

    public Task<bool> DeleteAsync(string id, CancellationToken cancellationToken = default) => _repository.DeleteAsync(id, cancellationToken);
}