using Microsoft.AspNetCore.Components;
using MyLittleLibrary.Domain;
using MudBlazor;
using MyLittleLibrary.Application.Queries;

namespace MyLittleLibrary.Components.Pages;

public partial class Home : ComponentBase
{
    [Inject] private IMangaQueryService MangaQueryService { get; set; } = null!;
    [Inject] private ILightNovelQueryService LightNovelQueryService { get; set; } = null!;
    [Inject] private IFilmQueryService FilmQueryService { get; set; } = null!;
    [Inject] private IBaseObjectQueryService BaseObjectQueryService { get; set; } = null!;
    [Inject] private NavigationManager NavigationManager { get; set; } = null!;
    [Inject] private IDialogService DialogService { get; set; } = null!;

    private string searchQuery = "";
    private int filmCount = 0;
    private int mangaSeriesCount = 0;
    private int mangaVolumeCount = 0;
    private int lightNovelSeriesCount = 0;
    private int lightNovelVolumeCount = 0;
    private int totalCount => filmCount + mangaVolumeCount + lightNovelVolumeCount;
    private List<BaseObject> recentItems = new();

    protected override async Task OnInitializedAsync()
    {
        await LoadStatistics();
        await LoadRecentItems();
    }

    private async Task LoadStatistics()
    {
        var filmList = await FilmQueryService.GetAllAsync();
        var mangaList = await MangaQueryService.GetAllAsync();
        var lightNovelList = await LightNovelQueryService.GetAllAsync();

        filmCount = filmList.Count;

        // For manga, count both series and total volumes
        var mangaGroups = mangaList.GroupBy(m => m.Title);
        mangaSeriesCount = mangaGroups.Count();
        mangaVolumeCount = mangaList.Count;

        // For light novels, count both series and total volumes
        var lightNovelGroups = lightNovelList.GroupBy(l => l.Title);
        lightNovelSeriesCount = lightNovelGroups.Count();
        lightNovelVolumeCount = lightNovelList.Count;
    }

    private async Task LoadRecentItems()
    {
        recentItems = await BaseObjectQueryService.GetMostRecentAsync();
    }

    private async Task PerformSearch()
    {
        if (!string.IsNullOrWhiteSpace(searchQuery))
        {
            // Navigate to the search results page with the query
            NavigationManager.NavigateTo($"/search?query={Uri.EscapeDataString(searchQuery)}");
        }
        else
        {
            // Optionally, you can show a message or handle empty search query
            await DialogService.ShowMessageBox("Search", "Please enter a search term.");
        }
    }

    private string GetItemType(Collection item)
    {
        return item switch
        {
            Collection.Manga => "Manga",
            Collection.LightNovel => "Light Novel",
            Collection.Book => "Book",
            Collection.Film => "Film",
            _ => "Unknown"
        };
    }
}