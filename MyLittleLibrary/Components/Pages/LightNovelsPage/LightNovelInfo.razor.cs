using Microsoft.AspNetCore.Components;
using MyLittleLibrary.Domain;
using MyLittleLibrary.Application;
using MudBlazor;

namespace MyLittleLibrary.Components.Pages.LightNovelsPage;

public partial class LightNovelInfo : ComponentBase
{
    [Parameter]
    [SupplyParameterFromQuery]
    public required string Title { get; set; }

    [Inject] private ILightNovelService LightNovelService { get; set; } = null!;
    [Inject] private IDialogService DialogService { get; set; } = null!;
    [Inject] private ISnackbar Snackbar { get; set; } = null!;

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
            var allLightNovels = await LightNovelService.GetAllByTitleAsync(Title);

            // Filter light novels by title (case-insensitive)
            lightNovelVolumes = allLightNovels
                .OrderBy(ln => ln.Volume)
                .ToList();
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

    private async Task OnChipClicked(Book.LightNovel lightNovel)
    {
        try
        {
            var updatedLightNovel = lightNovel with { IsRead = !lightNovel.IsRead };
            await LightNovelService.UpdateAsync(updatedLightNovel.Id, updatedLightNovel);
            await LoadLightNovelVolumes();
            Snackbar.Add($"Light novel marked as {(!lightNovel.IsRead ? "read" : "unread")}", Severity.Success);
        }
        catch (Exception)
        {
            Snackbar.Add("Error updating light novel status", Severity.Error);
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
            Snackbar.Add("Light novel successfully updated!", Severity.Success);
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
                Snackbar.Add("Light novel successfully added!", Severity.Success);
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
}