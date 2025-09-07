using MyLittleLibrary.Domain;

namespace MyLittleLibrary.Application.Commands;

public interface IFilmCommandService
{
    Task<Video.Film> CreateAsync(Video.Film film, CancellationToken cancellationToken = default);
    Task<bool> UpdateAsync(string id, Video.Film updatedFilm, CancellationToken cancellationToken = default);
    Task<bool> DeleteAsync(string id, CancellationToken cancellationToken = default);
}