using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Web;
using MyLittleLibrary.Domain;
using MyLittleLibrary.Application;
using MyLittleLibrary.Components.Shared;
using MudBlazor;

namespace MyLittleLibrary.Components.Pages.MangasPage;

public partial class ManageMangaCollections : ComponentBase
{
    [Inject] private IMangaService MangaService { get; set; } = null!;
    [Inject] private IDialogService DialogService { get; set; } = null!;
    [Inject] private ISnackbar Snackbar { get; set; } = null!;
    [Inject] private IWebHostEnvironment Environment { get; set; } = null!;

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
        allMangas = await MangaService.GetAllAsync();
        isLoading = false;
    }

    private async Task SearchMangas()
    {
        isLoading = true;
        
        if (string.IsNullOrWhiteSpace(searchQuery))
        {
            allMangas = await MangaService.GetAllAsync();
        }
        else
        {
            allMangas = await MangaService.SearchByTitleAsync(searchQuery);
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
            var success = await MangaService.DeleteAsync(id);
            var successImage = DeleteImageFile(imagePath);
            if (success)
            {
                Snackbar.Add("Manga deleted successfully", Severity.Success);
                await LoadMangas();
            }
            else
            {
                Snackbar.Add("Failed to delete manga", Severity.Error);
            }
        }
        catch (Exception ex)
        {
            Snackbar.Add($"Error: {ex.Message}", Severity.Error);
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
                var success = await MangaService.DeleteAsync(manga.Id);
                var successImage = DeleteImageFile(manga.ImagePath);
                if (success) deleteCount++;
            }

            if (deleteCount > 0)
            {
                Snackbar.Add($"Successfully deleted {deleteCount} manga volumes", Severity.Success);
                await LoadMangas();
            }
            else
            {
                Snackbar.Add("No manga volumes were deleted", Severity.Warning);
            }
        }
        catch (Exception ex)
        {
            Snackbar.Add($"Error: {ex.Message}", Severity.Error);
        }
    }

    private async Task DeleteAllMangas()
    {
        try
        {
            int deleteCount = 0;
            
            foreach (var manga in allMangas)
            {
                var success = await MangaService.DeleteAsync(manga.Id);
                var successImage = DeleteImageFile(manga.ImagePath);
                if (success) deleteCount++;
            }

            if (deleteCount > 0)
            {
                Snackbar.Add($"Successfully deleted {deleteCount} manga volumes", Severity.Success);
                await LoadMangas();
            }
            else
            {
                Snackbar.Add("No manga volumes were deleted", Severity.Warning);
            }
        }
        catch (Exception ex)
        {
            Snackbar.Add($"Error: {ex.Message}", Severity.Error);
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
}