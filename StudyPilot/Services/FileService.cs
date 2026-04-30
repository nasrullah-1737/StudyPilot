using StudyPilot.Models;
using StudyPilot.Services.Interfaces;

namespace StudyPilot.Services;

public class FileService : IFileService
{
    private readonly IDatabaseService _databaseService;
    private readonly string _storageRoot;

    public FileService(IDatabaseService databaseService)
    {
        _databaseService = databaseService;
        _storageRoot = Path.Combine(FileSystem.AppDataDirectory, "files");
        Directory.CreateDirectory(_storageRoot);
    }

    public async Task<IReadOnlyList<StudyFileItem>> GetAllAsync()
    {
        await _databaseService.InitializeAsync();
        return await _databaseService.Connection.Table<StudyFileItem>()
            .OrderByDescending(x => x.CreatedAt)
            .ToListAsync();
    }

    public async Task<StudyFileItem?> PickAndStoreAsync()
    {
        var picked = await FilePicker.Default.PickAsync(new PickOptions
        {
            PickerTitle = "Pick a PDF or image"
        });

        if (picked is null)
        {
            return null;
        }

        var extension = Path.GetExtension(picked.FileName).ToLowerInvariant();
        var allowedExtensions = new[] { ".pdf", ".png", ".jpg", ".jpeg", ".gif", ".bmp", ".webp" };
        if (!allowedExtensions.Contains(extension))
        {
            await Shell.Current.DisplayAlert("Unsupported File", "Please select a PDF or image file.", "OK");
            return null;
        }

        var fileName = $"{Guid.NewGuid()}_{picked.FileName}";
        var destination = Path.Combine(_storageRoot, fileName);

        await using var sourceStream = await picked.OpenReadAsync();
        await using var destinationStream = File.Create(destination);
        await sourceStream.CopyToAsync(destinationStream);

        var item = new StudyFileItem
        {
            DisplayName = picked.FileName,
            FilePath = destination,
            FileType = extension.TrimStart('.')
        };

        await _databaseService.InitializeAsync();
        await _databaseService.Connection.InsertAsync(item);
        return item;
    }

    public async Task OpenAsync(StudyFileItem fileItem)
    {
        if (!File.Exists(fileItem.FilePath))
        {
            await Shell.Current.DisplayAlert("File Missing", "File was not found on disk.", "OK");
            return;
        }

        await Launcher.Default.OpenAsync(new OpenFileRequest
        {
            File = new ReadOnlyFile(fileItem.FilePath)
        });
    }

    public async Task DeleteAsync(StudyFileItem fileItem)
    {
        await _databaseService.InitializeAsync();
        await _databaseService.Connection.DeleteAsync(fileItem);
        if (File.Exists(fileItem.FilePath))
        {
            File.Delete(fileItem.FilePath);
        }
    }
}
