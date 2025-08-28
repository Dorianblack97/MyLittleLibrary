using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Forms;
using MyLittleLibrary.Domain;
using MudBlazor;
using MyLittleLibrary.Application.Commands;

namespace MyLittleLibrary.Components.Pages.FilmsPage;

public partial class FilmDialog : ComponentBase
{
    [CascadingParameter] IMudDialogInstance MudDialog { get; set; } = null!;
    
    [Parameter] public Video.Film? FilmToEdit { get; set; }

    [Inject] private IFilmCommandService FilmCommandService { get; set; } = null!;
    [Inject] private ISnackbar Snackbar { get; set; } = null!;
    [Inject] private IWebHostEnvironment Environment { get; set; } = null!;

    private FilmModel film = new();
    private bool isSubmitting = false;

    // File upload fields
    private IBrowserFile? uploadedFile;
    private string originalFileExtension = string.Empty;
    private bool hasUploadErrors = false;
    private string uploadErrorMessage = string.Empty;

    private void Cancel() => MudDialog.Cancel();

    protected override void OnInitialized()
    {
        film = new FilmModel();
        
        if (FilmToEdit is not null)
        {
            film.Title = FilmToEdit.Title;
            film.Director = FilmToEdit.Director;
            film.Format = FilmToEdit.Format;
            film.ReleaseDate = FilmToEdit.ReleaseDate;
            film.ImagePath = FilmToEdit.ImagePath;
            film.IsWatched = FilmToEdit.IsWatched;
        }
    }

    private void UploadFilmCover(IReadOnlyList<IBrowserFile> files)
    {
        // Reset error message
        uploadErrorMessage = string.Empty;
        hasUploadErrors = false;
        
        // Single upload mode
        if (files.Count > 0)
        {
            uploadedFile = files[0];
            originalFileExtension = Path.GetExtension(files[0].Name);
            UpdateImagePath();
        }
    }

    private void UpdateImagePath()
    {
        if (uploadedFile is null || string.IsNullOrEmpty(film.Title)) return;
        
        // Create a title slug for the file path
        var titleSlug = film.Title.ToLowerInvariant()
            .Replace(" ", "-")
            .Replace("'", "")
            .Replace(":", "")
            .Replace(".", "");
        
        var fileName = $"{titleSlug}{originalFileExtension}";
        film.ImagePath = Path.Combine("images", "films", fileName);
    }

    private void ClearSelectedImage()
    {
        film.ImagePath = null;
        uploadedFile = null;
        originalFileExtension = string.Empty;
    }

    private async Task SaveImageToFilePath(IBrowserFile file, string imagePath)
    {
        var filePath = Path.Combine(Environment.WebRootPath, imagePath);
        
        // Ensure directory exists
        Directory.CreateDirectory(Path.GetDirectoryName(filePath)!);
        
        // Save the file to disk
        await using var fileStream = new FileStream(filePath, FileMode.Create);
        await file.OpenReadStream(maxAllowedSize: 10485760).CopyToAsync(fileStream);
    }

    // Modify the existing Submit method to handle the uploaded file
    private async Task Submit()
    {
        if (!IsValid()) return;
        
        isSubmitting = true;
        
        try
        {
            var titleSlug = film.Title.ToLowerInvariant()
                .Replace(" ", "-")
                .Replace("'", "")
                .Replace(":", "")
                .Replace(".", "");
        
            // Process the uploaded image if available
            if (uploadedFile != null)
            {
                // Save the image to disk
                await SaveImageToFilePath(uploadedFile, film.ImagePath);
            }

            if (FilmToEdit is null)
            {
                // Create new film
                var newFilm = new Video.Film(
                    title: film.Title,
                    titleSlug: titleSlug,
                    director: film.Director,
                    imagePath: string.IsNullOrWhiteSpace(film.ImagePath) ? null : film.ImagePath,
                    format: film.Format,
                    isWatched: film.IsWatched,
                    releaseDate: film.ReleaseDate
                );

                await FilmCommandService.CreateAsync(newFilm);
            }
            else
            {
                // Update existing film
                var updatedFilm = new Video.Film(
                    title: film.Title,
                    titleSlug: titleSlug,
                    director: film.Director,
                    imagePath: string.IsNullOrWhiteSpace(film.ImagePath) ? null : film.ImagePath,
                    format: film.Format,
                    isWatched: film.IsWatched,
                    releaseDate: film.ReleaseDate,
                    id: FilmToEdit.Id
                );

                await FilmCommandService.UpdateAsync(FilmToEdit.Id, updatedFilm);
            }
        
            MudDialog.Close(DialogResult.Ok(true));
        }
        catch (Exception ex)
        {
            Snackbar.Add($"Error {(FilmToEdit is null ? "adding" : "updating")} film: {ex.Message}", Severity.Error);
        }
        finally
        {
            isSubmitting = false;
        }
    }

    private bool IsValid()
    {
        return !string.IsNullOrWhiteSpace(film.Title) && 
               !string.IsNullOrWhiteSpace(film.Director);
    }

    public class FilmModel
    {
        public string Title { get; set; } = string.Empty;
        public string Director { get; set; } = string.Empty;
        public VideoFormat Format { get; set; } = VideoFormat.Digital;
        public DateTime? ReleaseDate { get; set; }
        public string? ImagePath { get; set; }
        public bool IsWatched { get; set; } = false;
    }
}