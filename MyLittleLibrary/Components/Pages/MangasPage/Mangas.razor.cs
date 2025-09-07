using Microsoft.AspNetCore.Components;
using MudBlazor;
using MyLittleLibrary.Application.Queries;

namespace MyLittleLibrary.Components.Pages.MangasPage;

public partial class Mangas : ComponentBase, IDisposable
{
    [Inject] private IMangaQueryService MangaQueryService { get; set; } = null!;
    [Inject] private NavigationManager NavigationManager { get; set; } = null!;
    [Inject] private ISnackbar Snackbar { get; set; } = null!;
    [Inject] private IDialogService DialogService { get; set; } = null!;

    private readonly CancellationTokenSource cancellationTokenSource = new();
    private bool isLoading = true;
    private List<MangaSeries> mangaSeries = new();
    private List<MangaSeries> filteredMangaSeries = new();
    private string searchString = string.Empty;

    protected override async Task OnInitializedAsync()
    {
        await LoadMangaCollection();
    }

    private async Task LoadMangaCollection()
    {
        isLoading = true;

        try
        {
            // Get all manga from the repository
            var allManga = await MangaQueryService.GetAllAsync(cancellationTokenSource.Token);

            // Group manga by title to create series
            mangaSeries = allManga
                .GroupBy(m => m.Title)
                .Select(group => new MangaSeries
                {
                    Title = group.Key,
                    Author = group.First().Author, // Assuming same author for all volumes
                    Illustrator = group.First().Illustrator, // Add this line
                    IsDigital = group.First().IsDigital,
                    VolumeCount = group.Count(),
                    CoverImage = group.OrderBy(m => m.Volume).LastOrDefault()?.ImagePath,
                    CompletionPercentage = (int)(group.Count(m => m.IsRead) * 100.0 / group.Count())
                })
                .OrderBy(s => s.Title)
                .ToList();

            // Initialize filtered list with all manga
            filteredMangaSeries = mangaSeries.ToList();
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
        FilterMangaSeries();
        StateHasChanged(); // Force UI update
    }

    // Filter manga series based on the search string
    private void FilterMangaSeries()
    {
        if (string.IsNullOrWhiteSpace(searchString))
        {
            // If search is empty, show all manga
            filteredMangaSeries = mangaSeries.ToList();
        }
        else
        {
            // Filter manga based on title, author, or illustrator
            var search = searchString.Trim().ToLowerInvariant();
            filteredMangaSeries = mangaSeries
                .Where(m =>
                    m.Title.ToLowerInvariant().Contains(search) ||
                    m.Author.ToLowerInvariant().Contains(search) ||
                    m.Illustrator.ToLowerInvariant().Contains(search))
                .ToList();
        }
    }

    private void NavigateToSeriesDetails(string title)
    {
        // Navigate to the MangaInfo page with the title as a query parameter
        NavigationManager.NavigateTo($"/manga/mangainfo?title={Uri.EscapeDataString(title)}");
    }

    private async Task OpenAddMangaDialog()
    {
        var options = new DialogOptions
        {
            CloseOnEscapeKey = true,
            FullWidth = true,
            MaxWidth = MaxWidth.Small,
            BackdropClick = false // Prevents accidental closing on mobile
        };

        var dialog = await DialogService.ShowAsync<MangaDialog>("Add New Manga", options);
        var result = await dialog.Result;

        if (!result.Canceled)
        {
            Snackbar.Add("Manga successfully added!", Severity.Success);
            // Force a reload with a slight delay to ensure file is fully saved
            await Task.Delay(100); // Small delay to ensure file system operations complete
            await LoadMangaCollection(); // Reload the manga list
            StateHasChanged(); // Force UI refresh
        }
    }

    // Update the MangaSeries class
    public class MangaSeries
    {
        public string Title { get; set; } = string.Empty;
        public string Author { get; set; } = string.Empty;
        public string Illustrator { get; set; } = string.Empty;
        public bool IsDigital { get; set; }
        public bool IsCompleted { get; set; } = false;
        public int VolumeCount { get; set; }
        public string? CoverImage { get; set; }
        public int CompletionPercentage { get; set; }
    }

    public void Dispose()
    {
        cancellationTokenSource?.Cancel();
        cancellationTokenSource?.Dispose();
    }
}