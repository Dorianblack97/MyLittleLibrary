using MyLittleLibrary.Domain;

namespace MyLittleLibrary.Infrastructure;

public class FilmService : IFilmService
{
    private readonly FilmRepository _repository;

    public FilmService(FilmRepository repository)
    {
        _repository = repository;
    }

    public Task<List<Video.Film>> GetAllAsync() => _repository.GetAllAsync();

    public Task<List<Video.Film>> GetAllByTitleAsync(string title) => _repository.GetAllByTitleAsync(title);

    public Task<Video.Film> GetByIdAsync(string id) => _repository.GetByIdAsync(id);

    public Task<Video.Film> GetByTitleAsync(string title) => _repository.GetByTitleAsync(title);

    public Task<List<Video.Film>> GetByDirectorAsync(string director) => _repository.GetByDirectorAsync(director);

    public Task<Video.Film> CreateAsync(Video.Film film) => _repository.CreateAsync(film);

    public Task<bool> UpdateAsync(string id, Video.Film updatedFilm) => _repository.UpdateAsync(id, updatedFilm);

    public Task<bool> DeleteAsync(string id) => _repository.DeleteAsync(id);

    public Task<List<Video.Film>> SearchByTitleAsync(string titleQuery) => _repository.SearchByTitleAsync(titleQuery);

    public Task<List<Video.Film>> SearchByDirectorAsync(string directorQuery) => _repository.SearchByDirectorAsync(directorQuery);

    public Task<List<Video.Film>> GetUnwatchedAsync() => _repository.GetUnwatchedAsync();

    public Task<List<Video.Film>> GetWatchedAsync() => _repository.GetWatchedAsync();

    public Task<List<Video.Film>> GetByFormatAsync(VideoFormat format) => _repository.GetByFormatAsync(format);
}