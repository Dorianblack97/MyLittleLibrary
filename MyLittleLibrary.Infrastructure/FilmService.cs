using MyLittleLibrary.Domain;
using MyLittleLibrary.Application;
using Microsoft.Extensions.Logging;

namespace MyLittleLibrary.Infrastructure;

public class FilmService : IFilmService
{
    private readonly FilmRepository _repository;
    private readonly ILogger<FilmService> _logger;

    public FilmService(FilmRepository repository, ILogger<FilmService> logger)
    {
        _repository = repository;
        _logger = logger;
    }

    public Task<List<Video.Film>> GetAllAsync(CancellationToken cancellationToken = default) => _repository.GetAllAsync(cancellationToken);

    public Task<List<Video.Film>> GetAllByTitleAsync(string title, CancellationToken cancellationToken = default) => _repository.GetAllByTitleAsync(title, cancellationToken);

    public Task<Video.Film> GetByIdAsync(string id, CancellationToken cancellationToken = default) => _repository.GetByIdAsync(id, cancellationToken);

    public Task<Video.Film> GetByTitleAsync(string title, CancellationToken cancellationToken = default) => _repository.GetByTitleAsync(title, cancellationToken);

    public Task<List<Video.Film>> GetByDirectorAsync(string director, CancellationToken cancellationToken = default) => _repository.GetByDirectorAsync(director, cancellationToken);

    public Task<Video.Film> CreateAsync(Video.Film film, CancellationToken cancellationToken = default) => _repository.CreateAsync(film, cancellationToken);

    public Task<bool> UpdateAsync(string id, Video.Film updatedFilm, CancellationToken cancellationToken = default) => _repository.UpdateAsync(id, updatedFilm, cancellationToken);

    public Task<bool> DeleteAsync(string id, CancellationToken cancellationToken = default) => _repository.DeleteAsync(id, cancellationToken);

    public Task<List<Video.Film>> SearchByTitleAsync(string titleQuery, CancellationToken cancellationToken = default) => _repository.SearchByTitleAsync(titleQuery, cancellationToken);

    public Task<List<Video.Film>> SearchByDirectorAsync(string directorQuery, CancellationToken cancellationToken = default) => _repository.SearchByDirectorAsync(directorQuery, cancellationToken);

    public Task<List<Video.Film>> GetUnwatchedAsync(CancellationToken cancellationToken = default) => _repository.GetUnwatchedAsync(cancellationToken);

    public Task<List<Video.Film>> GetWatchedAsync(CancellationToken cancellationToken = default) => _repository.GetWatchedAsync(cancellationToken);

    public Task<List<Video.Film>> GetByFormatAsync(VideoFormat format, CancellationToken cancellationToken = default) => _repository.GetByFormatAsync(format, cancellationToken);
}