using Microsoft.AspNetCore.Components;
using MyLittleLibrary.Domain;
using MudBlazor;
using MyLittleLibrary.Application.Queries;

namespace MyLittleLibrary.Components.Pages.FilmsPage;

public partial class Films : ComponentBase
{
    [Inject] private IFilmQueryService FilmQueryService { get; set; } = null!;
    [Inject] private NavigationManager NavigationManager { get; set; } = null!;
    [Inject] private ISnackbar Snackbar { get; set; } = null!;
    [Inject] private IDialogService DialogService { get; set; } = null!;

    private bool isLoading = true;
    private List<FilmModel> films = new();
    private List<FilmModel> filteredFilms = new();
    private string searchString = string.Empty;

    protected override async Task OnInitializedAsync()
    {
        await LoadFilmCollection();
    }

    private async Task LoadFilmCollection()
    {
        isLoading = true;

        try
        {
            // Get all films from the repository
            var allFilms = await FilmQueryService.GetAllAsync();

            // Convert to FilmModel for display
            films = allFilms
                .Select(f => new FilmModel
                {
                    Title = f.Title,
                    Director = f.Director,
                    Format = f.Format,
                    ReleaseDate = f.ReleaseDate,
                    IsWatched = f.IsWatched,
                    CoverImage = f.ImagePath
                })
                .OrderBy(f => f.Title)
                .ToList();

            // Initialize filtered list with all films
            filteredFilms = films.ToList();
        }
        catch (Exception)
        {
            // Handle any exceptions
        }
        finally
        {
            isLoading = false;
        }
    }

    // Handle search text changes
    private void SearchValueChanged(string value)
    {
        searchString = value;
        FilterFilms();
        StateHasChanged(); // Force UI update
    }

    // Filter films based on the search string
    private void FilterFilms()
    {
        if (string.IsNullOrWhiteSpace(searchString))
        {
            // If search is empty, show all films
            filteredFilms = films.ToList();
        }
        else
        {
            // Filter films based on title or director
            var search = searchString.Trim().ToLowerInvariant();
            filteredFilms = films
                .Where(f =>
                    f.Title.ToLowerInvariant().Contains(search) ||
                    f.Director.ToLowerInvariant().Contains(search))
                .ToList();
        }
    }

    private void NavigateToFilmDetails(string title)
    {
        // Navigate to the FilmInfo page with the title as a query parameter
        NavigationManager.NavigateTo($"/film/filminfo?title={Uri.EscapeDataString(title)}");
    }

    private async Task OpenAddFilmDialog()
    {
        var options = new DialogOptions
        {
            CloseOnEscapeKey = true,
            FullWidth = true,
            MaxWidth = MaxWidth.Small,
            BackdropClick = false // Prevents accidental closing on mobile
        };

        var dialog = await DialogService.ShowAsync<FilmDialog>("Add New Film", options);
        var result = await dialog.Result;

        if (!result.Canceled)
        {
            Snackbar.Add("Film successfully added!", Severity.Success);
            // Force a reload with a slight delay to ensure file is fully saved
            await Task.Delay(100); // Small delay to ensure file system operations complete
            await LoadFilmCollection(); // Reload the film list
            StateHasChanged(); // Force UI refresh
        }
    }

    // Helper class to represent a film based on Video.Film
    public class FilmModel
    {
        public string Title { get; set; } = string.Empty;
        public string Director { get; set; } = string.Empty;
        public VideoFormat Format { get; set; }
        public bool IsWatched { get; set; }
        public DateTime? ReleaseDate { get; set; }
        public string? CoverImage { get; set; }
    }
}