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

    public Task<Video.Film> CreateAsync(Video.Film film) => _repository.CreateAsync(film);

    public Task<bool> UpdateAsync(string id, Video.Film updatedFilm) => _repository.UpdateAsync(id, updatedFilm);

    public Task<bool> DeleteAsync(string id) => _repository.DeleteAsync(id);
}