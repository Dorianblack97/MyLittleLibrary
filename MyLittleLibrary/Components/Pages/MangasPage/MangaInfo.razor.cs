using Microsoft.AspNetCore.Components;
using MyLittleLibrary.Domain;
using MudBlazor;
using MyLittleLibrary.Application.Commands;
using MyLittleLibrary.Application;
using MyLittleLibrary.Application.Queries;

namespace MyLittleLibrary.Components.Pages.MangasPage;

public partial class MangaInfo : ComponentBase, IDisposable
{
    [Parameter] [SupplyParameterFromQuery] public required string Title { get; set; }

    [Inject] private IMangaQueryService MangaQueryService { get; set; } = null!;
    [Inject] private IMangaCommandService MangaCommandService { get; set; } = null!;
    [Inject] private IDialogService DialogService { get; set; } = null!;
    [Inject] private INotificationService Notifications { get; set; } = null!;
    [Inject] private ILogger<MangaInfo> Logger { get; set; } = null!;

    private readonly CancellationTokenSource cancellationTokenSource = new();
    private List<Book.Manga> mangaVolumes = new();
    private bool isLoading = true;
    private bool showImagePreview = false;
    private string previewImagePath = string.Empty;

    protected override async Task OnInitializedAsync()
    {
        await LoadMangaVolumes();
    }

    protected override async Task OnParametersSetAsync()
    {
        await LoadMangaVolumes();
    }

    private async Task LoadMangaVolumes()
    {
        isLoading = true;

        try
        {
            // Fetch all manga volumes for this series from the repository
            var allMangas = await MangaQueryService.GetAllByTitleAsync(Title, cancellationTokenSource.Token);

            // Filter mangas by title (case-insensitive)
            mangaVolumes = allMangas
                .OrderBy(m => m.Volume)
                .ToList();
        }
        catch (Exception ex)
        {
            // Handle any exceptions
            Logger.LogError(ex, "Error loading manga volumes");       
        }
        finally
        {
            isLoading = false;
        }
    }

    private async Task OnChipClicked(Book.Manga manga)
    {
        try
        {
            var updatedManga = manga with { IsRead = !manga.IsRead };
            await MangaCommandService.UpdateAsync(updatedManga.Id, updatedManga, cancellationTokenSource.Token);
            await LoadMangaVolumes();
            Notifications.Success($"Manga marked as {(!manga.IsRead ? "read" : "unread")}");
        }
        catch (Exception ex)
        {
            Notifications.Error("Error updating manga status");
            Logger.LogError(ex, "Error updating manga status");       
        }
    }

    private async Task OpenEditDialog(Book.Manga manga)
    {
        var parameters = new DialogParameters
        {
            ["MangaToEdit"] = manga,
            ["IsEdit"] = true
        };

        var options = new DialogOptions
        {
            CloseOnEscapeKey = true,
            FullWidth = true,
            MaxWidth = MaxWidth.Small,
            BackdropClick = false // Prevents accidental closing on mobile
        };

        var dialog = await DialogService.ShowAsync<MangaDialog>("Edit Manga", parameters, options);
        var result = await dialog.Result;

        if (!result.Canceled)
        {
            Notifications.Success("Manga successfully updated!");
            await Task.Delay(100); // Small delay to ensure file system operations complete
            await LoadMangaVolumes();
            StateHasChanged(); // Force UI refresh
        }
    }

    private async Task OpenAddVolumeDialog()
    {
        // Create a new manga object with the title pre-filled
        var mangaBase = mangaVolumes.Last();
        var newManga = new Book.Manga(
            title: mangaBase.Title,
            titleSlug: mangaBase.TitleSlug,
            author: mangaBase.Author,
            illustrator: mangaBase.Illustrator,
            volume: mangaBase.Volume + 1,
            imagePath: null,
            isDigital: mangaBase.IsDigital,
            isRead: false,
            publishDate: null);

        var parameters = new DialogParameters
        {
            ["MangaToEdit"] = newManga,
            ["IsEdit"] = false
        };

        var options = new DialogOptions
        {
            CloseOnEscapeKey = true,
            FullWidth = true,
            MaxWidth = MaxWidth.Small,
            BackdropClick = false
        };

        var dialog = await DialogService.ShowAsync<MangaDialog>("Add New Volume", parameters, options);
        var result = await dialog.Result;

        if (!result.Canceled)
        {
            Notifications.Success("Manga successfully added!");
            await Task.Delay(100); // Small delay to ensure file system operations complete
            await LoadMangaVolumes();
            StateHasChanged(); // Force UI refresh
        }
    }

    private void OpenImagePreview(string imagePath)
    {
        if (!string.IsNullOrEmpty(imagePath))
        {
            previewImagePath = imagePath;
            showImagePreview = true;
        }
    }

    private void CloseImagePreview()
    {
        showImagePreview = false;
    }

    public void Dispose()
    {
        cancellationTokenSource?.Cancel();
        cancellationTokenSource?.Dispose();
    }
}