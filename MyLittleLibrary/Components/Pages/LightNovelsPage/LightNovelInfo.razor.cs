using Microsoft.AspNetCore.Components;
using MyLittleLibrary.Domain;
using MudBlazor;
using MyLittleLibrary.Application.Commands;
using MyLittleLibrary.Application;
using MyLittleLibrary.Application.Queries;

namespace MyLittleLibrary.Components.Pages.LightNovelsPage;

public partial class LightNovelInfo : ComponentBase, IDisposable
{
    [Parameter]
    [SupplyParameterFromQuery]
    public required string Title { get; set; }

    [Inject] private ILightNovelQueryService LightNovelQueryService { get; set; } = null!;
    [Inject] private ILightNovelCommandService LightNovelCommandService { get; set; } = null!;
    [Inject] private IDialogService DialogService { get; set; } = null!;
    [Inject] private INotificationService Notifications { get; set; } = null!;
    [Inject] private ILogger<LightNovelInfo> Logger { get; set; } = null!;

    private readonly CancellationTokenSource cancellationTokenSource = new();
    private List<Book.LightNovel> lightNovelVolumes = new();
    private bool isLoading = true;
    private bool showImagePreview = false;
    private string previewImagePath = string.Empty;

    protected override async Task OnInitializedAsync()
    {
        await LoadLightNovelVolumes();
    }

    protected override async Task OnParametersSetAsync()
    {
        await LoadLightNovelVolumes();
    }

    private async Task LoadLightNovelVolumes()
    {
        isLoading = true;

        try
        {
            // Fetch all light novel volumes for this series from the repository
            var allLightNovels = await LightNovelQueryService.GetAllByTitleAsync(Title, cancellationTokenSource.Token);

            // Filter light novels by title (case-insensitive)
            lightNovelVolumes = allLightNovels
                .OrderBy(ln => ln.Volume)
                .ToList();
        }
        catch (Exception ex)
        {
            // Handle any exceptions
            Logger.LogError(ex, "Error loading light novel volumes");      
        }
        finally
        {
            isLoading = false;
        }
    }

    private async Task OnChipClicked(Book.LightNovel lightNovel)
    {
        try
        {
            var updatedLightNovel = lightNovel with { IsRead = !lightNovel.IsRead };
            await LightNovelCommandService.UpdateAsync(updatedLightNovel.Id, updatedLightNovel, cancellationTokenSource.Token);
            await LoadLightNovelVolumes();
            Notifications.Success($"Light novel marked as {(!lightNovel.IsRead ? "read" : "unread")}");
        }
        catch (Exception ex)
        {
            Notifications.Error("Error updating light novel status");
            Logger.LogError(ex, "Error updating light novel status");      
        }
    }

    private async Task OpenEditDialog(Book.LightNovel lightNovel)
    {
        var parameters = new DialogParameters
        {
            ["LightNovelToEdit"] = lightNovel,
            ["IsEdit"] = true
        };

        var options = new DialogOptions
        {
            CloseOnEscapeKey = true,
            FullWidth = true,
            MaxWidth = MaxWidth.Small,
            BackdropClick = false // Prevents accidental closing on mobile
        };

        var dialog = await DialogService.ShowAsync<LightNovelDialog>("Edit Light Novel", parameters, options);
        var result = await dialog.Result;

        if (!result.Canceled)
        {
            Notifications.Success("Light novel successfully updated!");
            await Task.Delay(100); // Small delay to ensure file system operations complete
            await LoadLightNovelVolumes();
            StateHasChanged(); // Force UI refresh
        }
    }

    private async Task OpenAddVolumeDialog()
    {
        // Check if there are any volumes
        if (lightNovelVolumes.Count > 0)
        {
            // Create a new light novel object with the title pre-filled
            var lightNovelBase = lightNovelVolumes.Last();
            var newLightNovel = new Book.LightNovel(
                title: lightNovelBase.Title,
                titleSlug: lightNovelBase.TitleSlug,
                author: lightNovelBase.Author,
                illustrator: lightNovelBase.Illustrator,
                volume: int.TryParse(lightNovelBase.Volume, out var number) ? (number + 1).ToString() : string.Empty,
                imagePath: null,
                isDigital: lightNovelBase.IsDigital,
                isRead: false,
                publishDate: null);

            var parameters = new DialogParameters
            {
                ["LightNovelToEdit"] = newLightNovel,
                ["IsEdit"] = false
            };

            var options = new DialogOptions
            {
                CloseOnEscapeKey = true,
                FullWidth = true,
                MaxWidth = MaxWidth.Small,
                BackdropClick = false
            };

            var dialog = await DialogService.ShowAsync<LightNovelDialog>("Add New Volume", parameters, options);
            var result = await dialog.Result;

            if (!result.Canceled)
            {
                Notifications.Success("Light novel successfully added!");
                await Task.Delay(100); // Small delay to ensure file system operations complete
                await LoadLightNovelVolumes();
                StateHasChanged(); // Force UI refresh
            }
        }
        else
        {
            // No volumes yet, open a blank dialog
            var options = new DialogOptions
            {
                CloseOnEscapeKey = true,
                FullWidth = true,
                MaxWidth = MaxWidth.Small,
                BackdropClick = false
            };

            var dialog = await DialogService.ShowAsync<LightNovelDialog>("Add New Light Novel", options);
            var result = await dialog.Result;

            if (!result.Canceled)
            {
                await LoadLightNovelVolumes();
            }
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