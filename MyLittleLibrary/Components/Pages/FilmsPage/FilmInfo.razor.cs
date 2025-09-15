using Microsoft.AspNetCore.Components;
using MyLittleLibrary.Domain;
using MyLittleLibrary.Components.Shared;
using MudBlazor;
using MyLittleLibrary.Application.Commands;
using MyLittleLibrary.Application;
using MyLittleLibrary.Application.Queries;

namespace MyLittleLibrary.Components.Pages.FilmsPage;

public partial class FilmInfo : ComponentBase, IDisposable
{
    [Parameter]
    [SupplyParameterFromQuery]
    public string? Title { get; set; }

    [Inject] private IFilmQueryService FilmQueryService { get; set; } = null!;
    [Inject] private IFilmCommandService FilmCommandService { get; set; } = null!;
    [Inject] private NavigationManager NavigationManager { get; set; } = null!;
    [Inject] private INotificationService Notifications { get; set; } = null!;
    [Inject] private IDialogService DialogService { get; set; } = null!;
    [Inject] private ILogger<FilmInfo> Logger { get; set; } = null!;

    private readonly CancellationTokenSource cancellationTokenSource = new();
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
            film = await FilmQueryService.GetByTitleAsync(Title!, cancellationTokenSource.Token);
        }
        catch (Exception ex)
        {
            // Handle exceptions
            Logger.LogError(ex, "Error loading film");
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
            await FilmCommandService.UpdateAsync(film.Id, updatedFilm, cancellationTokenSource.Token);
            film = updatedFilm;
            
            Notifications.Success($"Film marked as {(film.IsWatched ? "watched" : "unwatched")}");
        }
        catch (Exception ex)
        {
            Notifications.Error("Error updating film status");
            Logger.LogError(ex, "Error updating film status");      
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
            Notifications.Success("Film updated successfully!");
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
                await FilmCommandService.DeleteAsync(film!.Id, cancellationTokenSource.Token);
                Notifications.Success("Film deleted successfully!");
                NavigationManager.NavigateTo("/film");
            }
            catch (Exception ex)
            {
                Notifications.Error("Error deleting film");
                Logger.LogError(ex, "Error deleting film");
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

    public void Dispose()
    {
        cancellationTokenSource?.Cancel();
        cancellationTokenSource?.Dispose();
    }
}