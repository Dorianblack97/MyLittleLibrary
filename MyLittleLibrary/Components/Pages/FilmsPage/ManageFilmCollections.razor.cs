using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Web;
using MyLittleLibrary.Domain;
using MyLittleLibrary.Components.Shared;
using MudBlazor;
using MyLittleLibrary.Application.Commands;
using MyLittleLibrary.Application.Queries;

namespace MyLittleLibrary.Components.Pages.FilmsPage;

public partial class ManageFilmCollections : ComponentBase, IDisposable
{
    [Inject] private IFilmQueryService FilmQueryService { get; set; } = null!;
    [Inject] private IFilmCommandService FilmCommandService { get; set; } = null!;
    [Inject] private IDialogService DialogService { get; set; } = null!;
    [Inject] private ISnackbar Snackbar { get; set; } = null!;
    [Inject] private NavigationManager NavigationManager { get; set; } = null!;

    private readonly CancellationTokenSource cancellationTokenSource = new();
    private string searchQuery = "";
    private VideoFormat? formatFilter = null;
    private List<Video.Film> allFilms = new();
    private List<Video.Film> filteredFilms = new();
    private bool isLoading = true;
    private int selectedFormatValue = -1;

    protected override async Task OnInitializedAsync()
    {
        await LoadFilms();
    }

    private async Task LoadFilms()
    {
        isLoading = true;
        try
        {
            allFilms = await FilmQueryService.GetAllAsync(cancellationTokenSource.Token);
            FilterFilms();
        }
        catch (Exception ex)
        {
            Snackbar.Add($"Error loading films: {ex.Message}", Severity.Error);
        }
        finally
        {
            isLoading = false;
        }
    }

    private void FilterFilms()
    {
        filteredFilms = allFilms
            .Where(f => 
                (string.IsNullOrWhiteSpace(searchQuery) || 
                 f.Title.Contains(searchQuery, StringComparison.OrdinalIgnoreCase) ||
                 f.Director.Contains(searchQuery, StringComparison.OrdinalIgnoreCase)) &&
                (selectedFormatValue == -1 || (int)f.Format == selectedFormatValue))
            .ToList();
    }

    private async Task SearchFilms()
    {
        if (string.IsNullOrWhiteSpace(searchQuery) && formatFilter == null)
        {
            await LoadFilms();
        }
        else
        {
            FilterFilms();
        }
    }

    private async Task OnSearchKeyDown(KeyboardEventArgs args)
    {
        if (args.Key == "Enter")
        {
            await SearchFilms();
        }
    }

    private void ViewFilmDetails(string title)
    {
        NavigationManager.NavigateTo($"/films/filminfo?title={Uri.EscapeDataString(title)}");
    }

    private async Task ToggleWatchedStatus(Video.Film film)
    {
        try
        {
            var updatedFilm = film with { IsWatched = !film.IsWatched };
            var success = await FilmCommandService.UpdateAsync(film.Id, updatedFilm, cancellationTokenSource.Token);
            
            if (success)
            {
                Snackbar.Add($"'{film.Title}' marked as {(updatedFilm.IsWatched ? "watched" : "unwatched")}", Severity.Success);
                await LoadFilms();
            }
            else
            {
                Snackbar.Add("Failed to update film status", Severity.Error);
            }
        }
        catch (Exception ex)
        {
            Snackbar.Add($"Error: {ex.Message}", Severity.Error);
        }
    }

    private async Task OpenAddFilmDialog()
    {
        var options = new DialogOptions
        {
            CloseOnEscapeKey = true,
            FullWidth = true,
            MaxWidth = MaxWidth.Small
        };

        var dialog = await DialogService.ShowAsync<FilmDialog>("Add New Film", options);
        var result = await dialog.Result;

        if (!result.Canceled)
        {
            await LoadFilms();
            Snackbar.Add("Film added successfully!", Severity.Success);
        }
    }

    private async Task OpenEditFilmDialog(Video.Film film)
    {
        var parameters = new DialogParameters { ["Film"] = film };
        var options = new DialogOptions
        {
            CloseOnEscapeKey = true,
            FullWidth = true,
            MaxWidth = MaxWidth.Small
        };

        var dialog = await DialogService.ShowAsync<FilmDialog>("Edit Film", parameters, options);
        var result = await dialog.Result;

        if (!result.Canceled)
        {
            await LoadFilms();
            Snackbar.Add("Film updated successfully!", Severity.Success);
        }
    }

    private async Task OpenDeleteFilmDialog(Video.Film film)
    {
        var parameters = new DialogParameters
        {
            { "ContentText", $"Are you sure you want to delete '{film.Title}'? This action cannot be undone." },
            { "ButtonText", "Delete" },
            { "Color", Color.Error }
        };

        var dialog = await DialogService.ShowAsync<ConfirmDialog>("Delete Film", parameters);
        var result = await dialog.Result;

        if (!result.Canceled)
        {
            await DeleteFilm(film.Id);
        }
    }

    private async Task OpenDeleteAllFilmsDialog()
    {
        var parameters = new DialogParameters
        {
            { "ContentText", "Are you sure you want to delete ALL films? This action cannot be undone." },
            { "ButtonText", "Delete All" },
            { "Color", Color.Error }
        };

        var dialog = await DialogService.ShowAsync<ConfirmDialog>("Delete All Films", parameters);
        var result = await dialog.Result;

        if (!result.Canceled)
        {
            await DeleteAllFilms();
        }
    }

    private async Task DeleteFilm(string id)
    {
        try
        {
            var success = await FilmCommandService.DeleteAsync(id, cancellationTokenSource.Token);
            if (success)
            {
                Snackbar.Add("Film deleted successfully", Severity.Success);
                await LoadFilms();
            }
            else
            {
                Snackbar.Add("Failed to delete film", Severity.Error);
            }
        }
        catch (Exception ex)
        {
            Snackbar.Add($"Error: {ex.Message}", Severity.Error);
        }
    }

    private async Task DeleteAllFilms()
    {
        try
        {
            int deleteCount = 0;
            
            foreach (var film in allFilms)
            {
                var success = await FilmCommandService.DeleteAsync(film.Id, cancellationTokenSource.Token);
                if (success) deleteCount++;
            }

            if (deleteCount > 0)
            {
                Snackbar.Add($"Successfully deleted {deleteCount} films", Severity.Success);
                await LoadFilms();
            }
            else
            {
                Snackbar.Add("No films were deleted", Severity.Warning);
            }
        }
        catch (Exception ex)
        {
            Snackbar.Add($"Error: {ex.Message}", Severity.Error);
        }
    }

    private Color GetFormatColor(VideoFormat format)
    {
        return format switch
        {
            VideoFormat.BluRay => Color.Primary,
            VideoFormat.BluRay4k => Color.Tertiary,
            VideoFormat.Dvd => Color.Secondary,
            VideoFormat.Digital => Color.Info,
            VideoFormat.Vhs => Color.Warning,
            _ => Color.Default
        };
    }

    private async Task FormatFilterChanged(int value)
    {
        if (value == -1)
        {
            formatFilter = null;
        }
        else
        {
            formatFilter = (VideoFormat)value;
        }
        
        await SearchFilms();
    }

    public void Dispose()
    {
        cancellationTokenSource?.Cancel();
        cancellationTokenSource?.Dispose();
    }
}