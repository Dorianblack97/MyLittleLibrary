using MyLittleLibrary.Domain;

namespace MyLittleLibrary.Application.Queries;

public interface IFilmQueryService
{
    Task<List<Video.Film>> GetAllAsync(CancellationToken cancellationToken = default);
    Task<List<Video.Film>> GetAllByTitleAsync(string title, CancellationToken cancellationToken = default);
    Task<Video.Film> GetByIdAsync(string id, CancellationToken cancellationToken = default);
    Task<Video.Film> GetByTitleAsync(string title, CancellationToken cancellationToken = default);
    Task<List<Video.Film>> GetByDirectorAsync(string director, CancellationToken cancellationToken = default);
    Task<List<Video.Film>> SearchByTitleAsync(string titleQuery, CancellationToken cancellationToken = default);
    Task<List<Video.Film>> SearchByDirectorAsync(string directorQuery, CancellationToken cancellationToken = default);
    Task<List<Video.Film>> GetUnwatchedAsync(CancellationToken cancellationToken = default);
    Task<List<Video.Film>> GetWatchedAsync(CancellationToken cancellationToken = default);
    Task<List<Video.Film>> GetByFormatAsync(VideoFormat format, CancellationToken cancellationToken = default);
}