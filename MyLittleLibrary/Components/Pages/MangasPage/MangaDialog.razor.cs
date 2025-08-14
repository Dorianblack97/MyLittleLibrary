using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Forms;
using MyLittleLibrary.Domain;
using MyLittleLibrary.Application;
using MyLittleLibrary.Components.Shared;

namespace MyLittleLibrary.Components.Pages.MangasPage;

public partial class MangaDialog : BookUploadDialog
{
    [Parameter] public Book.Manga? MangaToEdit { get; set; }

    [Inject] private IMangaService MangaService { get; set; } = null!;

    private MangaMutable manga = new();

    protected override void InitializeData()
    {
        // Initialize with default or existing manga data
        manga = new MangaMutable();
        Book = manga;
        BookType = "Manga";
        
        if (MangaToEdit is not null)
        {
            // Store original values
            originalId = MangaToEdit.Id;
            
            // Initialize form with existing manga data
            manga.SetTitle(MangaToEdit.Title);
            manga.Author = MangaToEdit.Author;
            manga.Illustrator = MangaToEdit.Illustrator;
            manga.Volume = MangaToEdit.Volume;
            manga.ImagePath = MangaToEdit.ImagePath;
            manga.SetTitleSlug(MangaToEdit.TitleSlug);
            publishDate = MangaToEdit.PublishDate;
            
            // Initialize switch values from existing manga
            isDigital = MangaToEdit.IsDigital;
            isRead = MangaToEdit.IsRead;
        }
    }
    
    protected override async Task UpdateSingle()
    {
        // Update image path one more time to ensure it uses latest title/volume
        if (uploadedFile != null)
        {
            var fileExtension = Path.GetExtension(uploadedFile.Name);
            var fileName = $"{manga.TitleSlug}_{manga.Volume}{fileExtension}";
            manga.ImagePath = Path.Combine("images", manga.TitleSlug, fileName);

            await SaveImageToFilePath(uploadedFile, manga.ImagePath);
        }

        // Convert from mutable model to immutable record
        var mangaRecord = new Book.Manga(
            manga.Title,
            manga.TitleSlug,
            manga.Author,
            manga.Illustrator,
            manga.Volume,
            manga.ImagePath,
            isDigital,
            isRead,
            publishDate,
            originalId
        );

        await MangaService.UpdateAsync(originalId, mangaRecord);
    }
    
    protected override async Task CreateSingle()
    {
        // Process the uploaded image if available
        if (uploadedFile != null)
        {
            var fileExtension = Path.GetExtension(uploadedFile.Name);
            var fileName = $"{manga.TitleSlug}_{manga.Volume}{fileExtension}";
            manga.ImagePath = Path.Combine("images", manga.TitleSlug, fileName);
            
            await SaveImageToFilePath(uploadedFile, manga.ImagePath);
        }
        
        // Create a new manga record
        var newManga = new Book.Manga(
            manga.Title,
            manga.TitleSlug,
            manga.Author,
            manga.Illustrator,
            manga.Volume,
            manga.ImagePath,
            isDigital,
            isRead,
            publishDate
        );
        
        await MangaService.CreateAsync(newManga);
    }
    
    protected override async Task CreateMultiple()
    {
        // Create a directory if it doesn't exist
        var dirPath = Path.Combine(Environment.WebRootPath, "images", manga.TitleSlug);
        Directory.CreateDirectory(dirPath);
        
        // Map of volume numbers to their respective image files
        var volumeImageMap = new Dictionary<int, IBrowserFile>();
        
        // Parse volume numbers from file names and map them to files
        foreach (var file in uploadedFiles.Where(f => !invalidImageNames.Contains(f.Name)))
        {
            var fileName = Path.GetFileNameWithoutExtension(file.Name);
            var expectedNamePattern = $"{manga.TitleSlug}_";
            
            if (fileName.StartsWith(expectedNamePattern))
            {
                var volumeStr = fileName.Substring(expectedNamePattern.Length);
                if (int.TryParse(volumeStr, out int volumeNumber) && 
                    volumeNumber >= startVolume && volumeNumber <= endVolume)
                {
                    volumeImageMap[volumeNumber] = file;
                }
            }
        }
        
        // Create a manga volume for each specified volume
        for (int volume = startVolume; volume <= endVolume; volume++)
        {
            string? imagePath = null;
            
            // Check if we have an image for this volume
            if (volumeImageMap.TryGetValue(volume, out var imageFile))
            {
                var fileExtension = fileExtensionMap[imageFile.Name];
                var fileName = $"{manga.TitleSlug}_{volume}{fileExtension}";
                var filePath = Path.Combine(dirPath, fileName);
                
                await using FileStream fs = new(filePath, FileMode.Create);
                await imageFile.OpenReadStream(maxAllowedSize: 10485760).CopyToAsync(fs);
                
                // Set the image path
                imagePath = Path.Combine("images", manga.TitleSlug, fileName);
            }
            
            // Create and save the manga volume
            var newManga = new Book.Manga(
                manga.Title,
                manga.TitleSlug,
                manga.Author,
                manga.Illustrator,
                volume,
                imagePath,
                isDigital,
                isRead,
                publishDate
            );
            
            await MangaService.CreateAsync(newManga);
        }
    }
}