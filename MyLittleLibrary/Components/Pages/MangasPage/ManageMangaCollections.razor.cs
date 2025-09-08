using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Web;
using MyLittleLibrary.Domain;
using MyLittleLibrary.Components.Shared;
using MudBlazor;
using MyLittleLibrary.Application;
using MyLittleLibrary.Application;
using MyLittleLibrary.Application.Commands;
using MyLittleLibrary.Application.Queries;

namespace MyLittleLibrary.Components.Pages.MangasPage;

public partial class ManageMangaCollections : ComponentBase, IDisposable
{
    [Inject] private IMangaQueryService MangaQueryService { get; set; } = null!;
    [Inject] private IMangaCommandService MangaCommandService { get; set; } = null!;
    [Inject] private IDialogService DialogService { get; set; } = null!;
    [Inject] private INotificationService Notifications { get; set; } = null!;
    [Inject] private IWebHostEnvironment Environment { get; set; } = null!;

    private readonly CancellationTokenSource cancellationTokenSource = new();
    private string searchQuery = "";
    private List<Book.Manga> allMangas = new();
    private bool isLoading = true;

    private IEnumerable<IGrouping<string, Book.Manga>> GroupedMangas => 
        allMangas.GroupBy(m => m.Title);

    protected override async Task OnInitializedAsync()
    {
        await LoadMangas();
    }

    private async Task LoadMangas()
    {
        isLoading = true;
        allMangas = await MangaQueryService.GetAllAsync(cancellationTokenSource.Token);
        isLoading = false;
    }

    private async Task SearchMangas()
    {
        isLoading = true;
        
        if (string.IsNullOrWhiteSpace(searchQuery))
        {
            allMangas = await MangaQueryService.GetAllAsync(cancellationTokenSource.Token);
        }
        else
        {
            allMangas = await MangaQueryService.SearchByTitleAsync(searchQuery, cancellationTokenSource.Token);;
        }
        
        isLoading = false;
    }

    private async Task OnSearchKeyDown(KeyboardEventArgs args)
    {
        if (args.Key == "Enter")
        {
            await SearchMangas();
        }
    }

    private async Task OpenDeleteMangaDialog(Book.Manga manga)
    {
        var parameters = new DialogParameters
        {
            { "ContentText", $"Are you sure you want to delete '{manga.Title}' Volume {manga.Volume}? This action cannot be undone." },
            { "ButtonText", "Delete" },
            { "Color", Color.Error }
        };

        var dialog = await DialogService.ShowAsync<ConfirmDialog>("Confirm Deletion", parameters);
        var result = await dialog.Result;

        if (!result.Canceled)
        {
            await DeleteManga(manga.Id, manga.ImagePath);
        }
    }

    private async Task OpenDeleteCollectionDialog(string title = null)
    {
        string message = title == null 
            ? "Are you sure you want to delete ALL manga collections? This action cannot be undone."
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
                await DeleteAllMangas();
            }
            else
            {
                await DeleteMangaCollection(title);
            }
        }
    }

    private async Task DeleteManga(string id, string imagePath)
    {
        try
        {
            var success = await MangaCommandService.DeleteAsync(id, cancellationTokenSource.Token);
            var successImage = DeleteImageFile(imagePath);
            if (success)
            {
                Notifications.Success("Manga deleted successfully");
                await LoadMangas();
            }
            else
            {
                Notifications.Error("Failed to delete manga");
            }
        }
        catch (Exception ex)
        {
            Notifications.Error(ex);
        }
    }

    private async Task DeleteMangaCollection(string title)
    {
        try
        {
            var mangasToDelete = allMangas.Where(m => m.Title == title).ToList();
            int deleteCount = 0;
            
            foreach (var manga in mangasToDelete)
            {
                var success = await MangaCommandService.DeleteAsync(manga.Id, cancellationTokenSource.Token);
                var successImage = DeleteImageFile(manga.ImagePath);
                if (success) deleteCount++;
            }

            if (deleteCount > 0)
            {
                Notifications.Success($"Successfully deleted {deleteCount} manga volumes");
                await LoadMangas();
            }
            else
            {
                Notifications.Warning("No manga volumes were deleted");
            }
        }
        catch (Exception ex)
        {
            Notifications.Error($"Error: {ex.Message}");
        }
    }

    private async Task DeleteAllMangas()
    {
        try
        {
            int deleteCount = 0;
            
            foreach (var manga in allMangas)
            {
                var success = await MangaCommandService.DeleteAsync(manga.Id, cancellationTokenSource.Token);
                var successImage = DeleteImageFile(manga.ImagePath);
                if (success) deleteCount++;
            }

            if (deleteCount > 0)
            {
                Notifications.Success($"Successfully deleted {deleteCount} manga volumes");
                await LoadMangas();
            }
            else
            {
                Notifications.Warning("No manga volumes were deleted");
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