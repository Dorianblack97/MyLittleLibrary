using Microsoft.AspNetCore.Components;
using MudBlazor;
using MyLittleLibrary.Application.Queries;

namespace MyLittleLibrary.Components.Pages.LightNovelsPage;

public partial class LightNovels : ComponentBase, IDisposable
{
    [Inject] private ILightNovelQueryService LightNovelQueryService { get; set; } = null!;
    [Inject] private NavigationManager NavigationManager { get; set; } = null!;
    [Inject] private ISnackbar Snackbar { get; set; } = null!;
    [Inject] private IDialogService DialogService { get; set; } = null!;

    private readonly CancellationTokenSource cancellationTokenSource = new();
    private bool isLoading = true;
    private List<LightNovelSeries> lightNovelSeries = new();
    private List<LightNovelSeries> filteredLightNovelSeries = new();
    private string searchString = string.Empty;

    protected override async Task OnInitializedAsync()
    {
        await LoadLightNovelCollection();
    }

    private async Task LoadLightNovelCollection()
    {
        isLoading = true;

        try
        {
            // Get all light novels from the repository
            var allLightNovels = await LightNovelQueryService.GetAllAsync(cancellationTokenSource.Token);

            // Group light novels by title to create series
            lightNovelSeries = allLightNovels
                .GroupBy(ln => ln.Title)
                .Select(group => new LightNovelSeries
                {
                    Title = group.Key,
                    Author = group.First().Author, // Assuming same author for all volumes
                    Illustrator = group.First().Illustrator, // Assuming same illustrator for all volumes
                    IsDigital = group.First().IsDigital,
                    VolumeCount = group.Count(),
                    CoverImage = group.OrderBy(ln => ln.Volume).LastOrDefault()?.ImagePath,
                    CompletionPercentage = (int)(group.Count(ln => ln.IsRead) * 100.0 / group.Count())
                })
                .OrderBy(s => s.Title)
                .ToList();

            // Initialize filtered list with all light novels
            filteredLightNovelSeries = lightNovelSeries.ToList();
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
        FilterLightNovelSeries();
        StateHasChanged(); // Force UI update
    }

    // Filter light novel series based on the search string
    private void FilterLightNovelSeries()
    {
        if (string.IsNullOrWhiteSpace(searchString))
        {
            // If search is empty, show all light novels
            filteredLightNovelSeries = lightNovelSeries.ToList();
        }
        else
        {
            // Filter light novels based on title, author, or illustrator
            var search = searchString.Trim().ToLowerInvariant();
            filteredLightNovelSeries = lightNovelSeries
                .Where(ln =>
                    ln.Title.ToLowerInvariant().Contains(search) ||
                    ln.Author.ToLowerInvariant().Contains(search) ||
                    ln.Illustrator.ToLowerInvariant().Contains(search))
                .ToList();
        }
    }

    private void NavigateToSeriesDetails(string title)
    {
        // Navigate to the LightNovelInfo page with the title as a query parameter
        NavigationManager.NavigateTo($"/lightnovel/lightnovelinfo?title={Uri.EscapeDataString(title)}");
    }

    private async Task OpenAddLightNovelDialog()
    {
        var options = new DialogOptions
        {
            CloseOnEscapeKey = true,
            FullWidth = true,
            MaxWidth = MaxWidth.Small,
            BackdropClick = false // Prevents accidental closing on mobile
        };

        var dialog = await DialogService.ShowAsync<LightNovelDialog>("Add New Light Novel", options);
        var result = await dialog.Result;

        if (!result.Canceled)
        {
            Snackbar.Add("Light Novel successfully added!", Severity.Success);
            // Force a reload with a slight delay to ensure file is fully saved
            await Task.Delay(100); // Small delay to ensure file system operations complete
            await LoadLightNovelCollection(); // Reload the light novel list
            StateHasChanged(); // Force UI refresh
        }
    }

    // Helper class to represent a light novel series
    public class LightNovelSeries
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