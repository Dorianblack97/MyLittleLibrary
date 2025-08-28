using MyLittleLibrary.Domain;

namespace MyLittleLibrary.Application.Commands;

public interface IFilmCommandService
{
    Task<Video.Film> CreateAsync(Video.Film film);
    Task<bool> UpdateAsync(string id, Video.Film updatedFilm);
    Task<bool> DeleteAsync(string id);
}