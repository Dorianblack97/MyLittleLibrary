using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Forms;
using MyLittleLibrary.Domain;
using MyLittleLibrary.Application.Commands;
using MyLittleLibrary.Components.Shared;

namespace MyLittleLibrary.Components.Pages.LightNovelsPage;

public partial class LightNovelDialog : BookUploadDialog, IDisposable
{
    [Parameter] public Book.LightNovel? LightNovelToEdit { get; set; }
    [Inject] private ILightNovelCommandService LightNovelCommandService { get; set; } = null!;

    private readonly CancellationTokenSource cancellationTokenSource = new();
    private LightNovelMutable lightNovel = new();

    protected override void InitializeData()
    {
        // Initialize with default or existing lightNovel data
        lightNovel = new LightNovelMutable();
        Book = lightNovel;
        BookType = "Light Novel";
        
        if (LightNovelToEdit is not null)
        {
            // Store original values
            originalId = LightNovelToEdit.Id;
            
            // Initialize form with existing lightNovel data
            lightNovel.SetTitle(LightNovelToEdit.Title);
            lightNovel.Author = LightNovelToEdit.Author;
            lightNovel.Illustrator = LightNovelToEdit.Illustrator;
            lightNovel.Volume = LightNovelToEdit.Volume;
            lightNovel.ImagePath = LightNovelToEdit.ImagePath;
            lightNovel.SetTitleSlug(LightNovelToEdit.TitleSlug);
            publishDate = LightNovelToEdit.PublishDate;
            
            // Initialize switch values from existing lightNovel
            isDigital = LightNovelToEdit.IsDigital;
            isRead = LightNovelToEdit.IsRead;
        }
    }
    
    protected override async Task UpdateSingle()
    {
        // Update image path one more time to ensure it uses latest title/volume
        if (uploadedFile != null)
        {
            var fileExtension = Path.GetExtension(uploadedFile.Name);
            var fileName = $"{lightNovel.TitleSlug}_{lightNovel.Volume}{fileExtension}";
            lightNovel.ImagePath = Path.Combine("images", lightNovel.TitleSlug, fileName);

            await SaveImageToFilePath(uploadedFile, lightNovel.ImagePath);
        }

        // Convert from mutable model to immutable record
        var lightNovelRecord = new Book.LightNovel(
            lightNovel.Title,
            lightNovel.TitleSlug,
            lightNovel.Author,
            lightNovel.Illustrator,
            lightNovel.Volume,
            lightNovel.ImagePath,
            isDigital,
            isRead,
            publishDate,
            originalId
        );

        await LightNovelCommandService.UpdateAsync(originalId, lightNovelRecord, cancellationTokenSource.Token);
    }
    
    protected override async Task CreateSingle()
    {
        // Process the uploaded image if available
        if (uploadedFile != null)
        {
            var fileExtension = Path.GetExtension(uploadedFile.Name);
            var fileName = $"{lightNovel.TitleSlug}_{lightNovel.Volume}{fileExtension}";
            lightNovel.ImagePath = Path.Combine("images", lightNovel.TitleSlug, fileName);
            
            await SaveImageToFilePath(uploadedFile, lightNovel.ImagePath);
        }
        
        // Create a new light novel record
        var newLightNovel = new Book.LightNovel(
            lightNovel.Title,
            lightNovel.TitleSlug,
            lightNovel.Author,
            lightNovel.Illustrator,
            lightNovel.Volume,
            lightNovel.ImagePath,
            isDigital,
            isRead,
            publishDate
        );
        
        await LightNovelCommandService.CreateAsync(newLightNovel, cancellationTokenSource.Token);
    }
    
    protected override async Task CreateMultiple()
    {
        // Create a directory if it doesn't exist
        var dirPath = Path.Combine(Environment.WebRootPath, "images", lightNovel.TitleSlug);
        Directory.CreateDirectory(dirPath);
        
        // Map of volume numbers to their respective image files
        var volumeImageMap = new Dictionary<int, IBrowserFile>();
        
        // Parse volume numbers from file names and map them to files
        foreach (var file in uploadedFiles.Where(f => !invalidImageNames.Contains(f.Name)))
        {
            var fileName = Path.GetFileNameWithoutExtension(file.Name);
            var expectedNamePattern = $"{lightNovel.TitleSlug}_";
            
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
        
        // Create a lightNovel volume for each specified volume
        for (int volume = startVolume; volume <= endVolume; volume++)
        {
            string? imagePath = null;
            
            // Check if we have an image for this volume
            if (volumeImageMap.TryGetValue(volume, out var imageFile))
            {
                var fileExtension = fileExtensionMap[imageFile.Name];
                var fileName = $"{lightNovel.TitleSlug}_{volume}{fileExtension}";
                var filePath = Path.Combine(dirPath, fileName);
                
                await using FileStream fs = new(filePath, FileMode.Create);
                await imageFile.OpenReadStream(maxAllowedSize: 10485760).CopyToAsync(fs);
                
                // Set the image path
                imagePath = Path.Combine("images", lightNovel.TitleSlug, fileName);
            }
            
            // Create and save the lightNovel volume
            var newLightNovel = new Book.LightNovel(
                lightNovel.Title,
                lightNovel.TitleSlug,
                lightNovel.Author,
                lightNovel.Illustrator,
                volume.ToString(),
                imagePath,
                isDigital,
                isRead,
                publishDate
            );
            
            await LightNovelCommandService.CreateAsync(newLightNovel, cancellationTokenSource.Token);
        }
    }
    
    public void Dispose()
    {
        cancellationTokenSource?.Cancel();
        cancellationTokenSource?.Dispose();
    }
}