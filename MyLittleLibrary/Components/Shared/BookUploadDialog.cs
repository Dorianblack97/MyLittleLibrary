using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Forms;
using MudBlazor;

namespace MyLittleLibrary.Components.Shared;

public abstract class BookUploadDialog : ComponentBase
{
    [Inject] protected IWebHostEnvironment Environment { get; set; }
    [Inject] protected IConfiguration Configuration { get; set; }

    [CascadingParameter] protected IMudDialogInstance MudDialog { get; set; }
        
    [Parameter] public bool IsEdit { get; set; } = false;
    [Parameter] public string BookType { get; set; } = "Book";
        
    protected BookMutable Book { get; set; }
    protected DateTime? publishDate;
    protected bool success;
    protected bool isSubmitting;
    protected MudForm form;
    protected IBrowserFile? uploadedFile;
    protected List<IBrowserFile> uploadedFiles = new();
    protected Dictionary<string, string> fileExtensionMap = new();
    protected string originalFileExtension = string.Empty;
    protected string originalId = string.Empty;
        
    // Multiple volume support
    protected bool isMultipleVolumes;
    protected int startVolume = 1;
    protected int endVolume = 1;
    protected HashSet<string> invalidImageNames = new();
        
    // Configurable file upload limit
    protected int maxFileUploadCount;
        
    // Store switch values in separate fields to better handle binding
    protected bool isDigital;
    protected bool isRead;
        
    // Error handling
    protected bool hasUploadErrors = false;
    protected string uploadErrorMessage = string.Empty;

    protected virtual void InitializeData() { }
        
    protected virtual Task UpdateSingle() => Task.CompletedTask;
    protected virtual Task CreateSingle() => Task.CompletedTask;
    protected virtual Task CreateMultiple() => Task.CompletedTask;

    protected override void OnInitialized()
    {
        // Get configurable file upload limit from appsettings.json with a default of 50
        maxFileUploadCount = Configuration.GetValue($"FileUpload:Max{BookType.Replace(" ", "")}Files", 50);
            
        InitializeData();
    }

    protected void Cancel()
    {
        MudDialog.Cancel();
    }

    protected void UploadFiles(IReadOnlyList<IBrowserFile> files)
    {
        // Reset error message
        uploadErrorMessage = string.Empty;
            
        if (IsEdit || !isMultipleVolumes)
        {
            // Single upload mode
            if (files.Count > 0)
            {
                uploadedFile = files[0];
                originalFileExtension = Path.GetExtension(files[0].Name);
                UpdateImagePath();
            }
        }
        else
        {
            // Multiple upload mode - additive approach
            if (uploadedFiles.Count + files.Count > maxFileUploadCount)
            {
                hasUploadErrors = true;
                uploadErrorMessage = $"Maximum {maxFileUploadCount} files allowed. Cannot add {files.Count} more files to existing {uploadedFiles.Count}.";
                return;
            }
                
            if (string.IsNullOrEmpty(Book.TitleSlug))
            {
                hasUploadErrors = true;
                uploadErrorMessage = "Please enter a title first to generate the title slug.";
                return;
            }
                
            foreach (var file in files)
            {
                // Skip if this file is already in the list (prevent duplicates)
                if (uploadedFiles.Any(f => f.Name == file.Name))
                    continue;
                    
                // Add file to the list
                uploadedFiles.Add(file);
                fileExtensionMap[file.Name] = Path.GetExtension(file.Name);
                    
                // Check if the file follows the naming convention
                var fileName = Path.GetFileNameWithoutExtension(file.Name);
                var expectedNamePattern = $"{Book.TitleSlug}_";
                    
                if (!fileName.StartsWith(expectedNamePattern))
                {
                    invalidImageNames.Add(file.Name);
                    continue;
                }
                    
                var volumeStr = fileName.Substring(expectedNamePattern.Length);
                if (!int.TryParse(volumeStr, out int volumeNumber) || 
                    volumeNumber < startVolume || volumeNumber > endVolume)
                {
                    invalidImageNames.Add(file.Name);
                }
            }
                
            if (invalidImageNames.Count > 0)
            {
                hasUploadErrors = true;
                uploadErrorMessage = $"{invalidImageNames.Count} file(s) don't match the required naming pattern: {Book.TitleSlug}_[VolumeNumber].";
            }
        }
    }

    protected void UpdateImagePath()
    {
        if (uploadedFile is null || string.IsNullOrEmpty(Book.TitleSlug)) return;
        var fileName = $"{Book.TitleSlug}_{Book.VolumeValue}{originalFileExtension}";
        Book.ImagePath = Path.Combine("images", Book.TitleSlug, fileName);
    }

    protected void ClearSelectedImage()
    {
        Book.ImagePath = null;
        uploadedFile = null;
        originalFileExtension = string.Empty;
    }
        
    protected void ClearAllSelectedImages()
    {
        uploadedFiles.Clear();
        fileExtensionMap.Clear();
        invalidImageNames.Clear();
        hasUploadErrors = false;
        uploadErrorMessage = string.Empty;
    }
        
    protected void RemoveUploadedFile(IBrowserFile file)
    {
        // Remove file from uploaded files list
        uploadedFiles.Remove(file);
            
        // Remove from extension map if it exists
        if (fileExtensionMap.ContainsKey(file.Name))
        {
            fileExtensionMap.Remove(file.Name);
        }
            
        // Remove from invalid images if it exists
        invalidImageNames.Remove(file.Name);
            
        // Update error message if needed
        if (invalidImageNames.Count > 0)
        {
            hasUploadErrors = true;
            uploadErrorMessage = $"{invalidImageNames.Count} file(s) don't match the required naming pattern.";
        }
        else
        {
            hasUploadErrors = false;
            uploadErrorMessage = string.Empty;
        }
    }

    protected async Task SaveImageToFilePath(IBrowserFile file, string imagePath)
    {
        var filePath = Path.Combine(Environment.WebRootPath, imagePath);
            
        // Ensure directory exists
        Directory.CreateDirectory(Path.GetDirectoryName(filePath)!);
            
        // Save the file to disk
        await using var fileStream = new FileStream(filePath, FileMode.Create);
        await file.OpenReadStream(maxAllowedSize: 10485760).CopyToAsync(fileStream);
    }

    protected async Task Submit()
    {
        await form.Validate();

        if (success)
        {
            isSubmitting = true;
            List<string> invalidImages = new List<string>(invalidImageNames);

            try
            {
                if (IsEdit)
                {
                    // Single update
                    await UpdateSingle();
                }
                else if (!isMultipleVolumes)
                {
                    // Create single
                    await CreateSingle();
                }
                else
                {
                    // Create multiple volumes
                    await CreateMultiple();
                }
                
                // Give the file system operations a moment to complete
                await Task.Delay(50);

                // Return result to the caller
                var result = new
                {
                    Success = true,
                    InvalidImages = invalidImages
                };
                
                MudDialog.Close(DialogResult.Ok(result));
            }
            catch (Exception ex)
            {
                isSubmitting = false;
                hasUploadErrors = true;
                uploadErrorMessage = $"Error saving {BookType.ToLower()}: {ex.Message}";
            }
        }
    }

    protected override async Task OnAfterRenderAsync(bool firstRender)
    {
        await form.Validate();
    }
}