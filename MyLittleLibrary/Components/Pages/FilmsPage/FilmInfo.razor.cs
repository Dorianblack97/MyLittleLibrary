using Microsoft.AspNetCore.Components;
using MyLittleLibrary.Domain;
using MyLittleLibrary.Application;
using MyLittleLibrary.Components.Shared;
using MudBlazor;

namespace MyLittleLibrary.Components.Pages.FilmsPage;

public partial class FilmInfo : ComponentBase
{
    [Parameter]
    [SupplyParameterFromQuery]
    public string? Title { get; set; }

    [Inject] private IFilmService FilmService { get; set; } = null!;
    [Inject] private NavigationManager NavigationManager { get; set; } = null!;
    [Inject] private ISnackbar Snackbar { get; set; } = null!;
    [Inject] private IDialogService DialogService { get; set; } = null!;

    private Video.Film? film;
    private bool isLoading = true;

    protected override async Task OnInitializedAsync()
    {
        if (!string.IsNullOrEmpty(Title))
        {
            await LoadFilm();
        }
        else
        {
            isLoading = false;
        }
    }

    private async Task LoadFilm()
    {
        isLoading = true;
        try
        {
            film = await FilmService.GetByTitleAsync(Title!);
        }
        catch (Exception)
        {
            // Handle exceptions
        }
        finally
        {
            isLoading = false;
        }
    }

    private async Task ToggleWatchedStatus()
    {
        if (film == null) return;

        try
        {
            var updatedFilm = film with { IsWatched = !film.IsWatched };
            await FilmService.UpdateAsync(film.Id, updatedFilm);
            film = updatedFilm;
            
            Snackbar.Add($"Film marked as {(film.IsWatched ? "watched" : "unwatched")}", Severity.Success);
        }
        catch (Exception)
        {
            Snackbar.Add("Error updating film status", Severity.Error);
        }
    }

    private async Task OpenEditDialog()
    {
        var parameters = new DialogParameters { ["FilmToEdit"] = film };
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
            await LoadFilm();
            Snackbar.Add("Film updated successfully!", Severity.Success);
        }
    }

    private async Task OpenDeleteDialog()
    {
        var parameters = new DialogParameters
        {
            ["ContentText"] = $"Are you sure you want to delete '{film?.Title}'? This action cannot be undone.",
            ["ButtonText"] = "Delete",
            ["Color"] = Color.Error
        };

        var dialog = await DialogService.ShowAsync<ConfirmDialog>("Delete Film", parameters);
        var result = await dialog.Result;

        if (!result.Canceled)
        {
            try
            {
                await FilmService.DeleteAsync(film!.Id);
                Snackbar.Add("Film deleted successfully!", Severity.Success);
                NavigationManager.NavigateTo("/film");
            }
            catch (Exception)
            {
                Snackbar.Add("Error deleting film", Severity.Error);
            }
        }
    }

    private string GetFormatIcon(VideoFormat format)
    {
        return format switch
        {
            VideoFormat.BluRay => Icons.Material.Filled.Album,
            VideoFormat.Dvd => Icons.Material.Filled.Album,
            VideoFormat.Digital => Icons.Material.Filled.CloudDownload,
            VideoFormat.Vhs => Icons.Material.Filled.Videocam,
            _ => Icons.Material.Filled.Movie
        };
    }
}