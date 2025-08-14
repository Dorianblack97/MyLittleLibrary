using MyLittleLibrary.Domain;

namespace MyLittleLibrary.Application;

public interface IFilmService
{
    Task<List<Video.Film>> GetAllAsync();
    Task<List<Video.Film>> GetAllByTitleAsync(string title);
    Task<Video.Film> GetByIdAsync(string id);
    Task<Video.Film> GetByTitleAsync(string title);
    Task<List<Video.Film>> GetByDirectorAsync(string director);
    Task<Video.Film> CreateAsync(Video.Film film);
    Task<bool> UpdateAsync(string id, Video.Film updatedFilm);
    Task<bool> DeleteAsync(string id);
    Task<List<Video.Film>> SearchByTitleAsync(string titleQuery);
    Task<List<Video.Film>> SearchByDirectorAsync(string directorQuery);
    Task<List<Video.Film>> GetUnwatchedAsync();
    Task<List<Video.Film>> GetWatchedAsync();
    Task<List<Video.Film>> GetByFormatAsync(VideoFormat format);
}