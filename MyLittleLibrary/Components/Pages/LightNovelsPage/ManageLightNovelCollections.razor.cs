using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Web;
using MyLittleLibrary.Domain;
using MyLittleLibrary.Components.Shared;
using MudBlazor;
using MyLittleLibrary.Application.Commands;
using MyLittleLibrary.Application.Queries;
using MyLittleLibrary.Application;

namespace MyLittleLibrary.Components.Pages.LightNovelsPage;

public partial class ManageLightNovelCollections : ComponentBase, IDisposable
{
    [Inject] private ILightNovelQueryService LightNovelQueryService { get; set; } = null!;
    [Inject] private ILightNovelCommandService LightNovelCommandService { get; set; } = null!;
    [Inject] private IDialogService DialogService { get; set; } = null!;
    [Inject] private INotificationService Notifications { get; set; } = null!;
    [Inject] private IWebHostEnvironment Environment { get; set; } = null!;

    private readonly CancellationTokenSource cancellationTokenSource = new();
    private string searchQuery = "";
    private List<Book.LightNovel> allLightNovels = new();
    private bool isLoading = true;

    private IEnumerable<IGrouping<string, Book.LightNovel>> GroupedLightNovels =>
        allLightNovels.GroupBy(ln => ln.Title);

    protected override async Task OnInitializedAsync()
    {
        await LoadLightNovels();
    }

    private async Task LoadLightNovels()
    {
        isLoading = true;
        allLightNovels = await LightNovelQueryService.GetAllAsync(cancellationTokenSource.Token);
        isLoading = false;
    }

    private async Task SearchLightNovels()
    {
        isLoading = true;

        if (string.IsNullOrWhiteSpace(searchQuery))
        {
            allLightNovels = await LightNovelQueryService.GetAllAsync(cancellationTokenSource.Token);
        }
        else
        {
            allLightNovels = await LightNovelQueryService.SearchByTitleAsync(searchQuery, cancellationTokenSource.Token);
        }

        isLoading = false;
    }

    private async Task OnSearchKeyDown(KeyboardEventArgs args)
    {
        if (args.Key == "Enter")
        {
            await SearchLightNovels();
        }
    }

    private async Task OpenDeleteLightNovelDialog(Book.LightNovel lightNovel)
    {
        var parameters = new DialogParameters
        {
            { "ContentText", $"Are you sure you want to delete '{lightNovel.Title}' Volume {lightNovel.Volume}? This action cannot be undone." },
            { "ButtonText", "Delete" },
            { "Color", Color.Error }
        };

        var dialog = await DialogService.ShowAsync<ConfirmDialog>("Confirm Deletion", parameters);
        var result = await dialog.Result;

        if (!result.Canceled)
        {
            await DeleteLightNovel(lightNovel.Id, lightNovel.ImagePath);
        }
    }

    private async Task OpenDeleteCollectionDialog(string title = null)
    {
        string message = title == null
            ? "Are you sure you want to delete ALL light novel collections? This action cannot be undone."
            : $"Are you sure you want to delete the entire '{title}' series? This action cannot be undone.";

        var parameters = new DialogParameters
        {
            { "ContentText", message },
            { "ButtonText", "Delete" },
            { "Color", Color.Error }
        };

        var dialog = await DialogService.ShowAsync<ConfirmDialog>("Confirm Deletion", parameters);
        var result = await dialog.Result;

        if (!result.Canceled)
        {
            if (title == null)
            {
                await DeleteAllLightNovels();
            }
            else
            {
                await DeleteLightNovelCollection(title);
            }
        }
    }

    private async Task DeleteLightNovel(string id, string imagePath)
    {
        try
        {
            var success = await LightNovelCommandService.DeleteAsync(id, cancellationTokenSource.Token);
            var successImage = DeleteImageFile(imagePath);
            if (success)
            {
                Notifications.Success("Light Novel deleted successfully");
                await LoadLightNovels();
            }
            else
            {
                Notifications.Error("Failed to delete light novel");
            }
        }
        catch (Exception ex)
        {
            Notifications.Error(ex);
        }
    }

    private async Task DeleteLightNovelCollection(string title)
    {
        try
        {
            var lightNovelsToDelete = allLightNovels.Where(ln => ln.Title == title).ToList();
            int deleteCount = 0;

            foreach (var lightNovel in lightNovelsToDelete)
            {
                var success = await LightNovelCommandService.DeleteAsync(lightNovel.Id, cancellationTokenSource.Token);
                var successImage = DeleteImageFile(lightNovel.ImagePath);
                if (success) deleteCount++;
            }

            if (deleteCount > 0)
            {
                Notifications.Success($"Successfully deleted {deleteCount} light novel volumes");
                await LoadLightNovels();
            }
            else
            {
                Notifications.Warning("No light novel volumes were deleted");
            }
        }
        catch (Exception ex)
        {
            Notifications.Error($"Error: {ex.Message}");
        }
    }

    private async Task DeleteAllLightNovels()
    {
        try
        {
            int deleteCount = 0;

            foreach (var lightNovel in allLightNovels)
            {
                var success = await LightNovelCommandService.DeleteAsync(lightNovel.Id, cancellationTokenSource.Token);
                var successImage = DeleteImageFile(lightNovel.ImagePath);
                if (success) deleteCount++;
            }

            if (deleteCount > 0)
            {
                Notifications.Success($"Successfully deleted {deleteCount} light novel volumes");
                await LoadLightNovels();
            }
            else
            {
                Notifications.Warning("No light novel volumes were deleted");
            }
        }
        catch (Exception ex)
        {
            Notifications.Error($"Error: {ex.Message}");
        }
    }

    private bool DeleteImageFile(string imagePath)
    {
        if (string.IsNullOrEmpty(imagePath))
            return false;

        try
        {
            var fullPath = Path.Combine(Environment.WebRootPath, imagePath);
            if (File.Exists(fullPath))
            {
                File.Delete(fullPath);

                // Check if directory is empty after deleting the file
                var directory = Path.GetDirectoryName(fullPath);
                if (directory != null && Directory.Exists(directory) && !Directory.EnumerateFiles(directory).Any())
                {
                    // Remove empty directory
                    Directory.Delete(directory);
                }

                return true;
            }
        }
        catch (Exception)
        {
            // Handle any file system exceptions
            return false;
        }

        return false;
    }

    public void Dispose()
    {
        cancellationTokenSource?.Cancel();
        cancellationTokenSource?.Dispose();
    }
}