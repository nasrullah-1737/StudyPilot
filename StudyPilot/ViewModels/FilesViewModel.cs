using CommunityToolkit.Mvvm.Input;
using StudyPilot.Models;
using StudyPilot.Services.Interfaces;
using System.Collections.ObjectModel;

namespace StudyPilot.ViewModels;

public partial class FilesViewModel : BaseViewModel
{
    private readonly IFileService _fileService;
    private readonly IAlertService _alertService;

    public ObservableCollection<StudyFileItem> Files { get; } = [];

    public FilesViewModel(IFileService fileService, IAlertService alertService)
    {
        _fileService = fileService;
        _alertService = alertService;
        Title = "File Storage";
    }

    [RelayCommand]
    private async Task LoadAsync()
    {
        if (IsBusy)
        {
            return;
        }

        IsBusy = true;
        try
        {
            Files.Clear();
            foreach (var item in await _fileService.GetAllAsync())
            {
                Files.Add(item);
            }
        }
        finally
        {
            IsBusy = false;
        }
    }

    [RelayCommand]
    private async Task UploadAsync()
    {
        var item = await _fileService.PickAndStoreAsync();
        if (item is not null)
        {
            Files.Insert(0, item);
        }
    }

    [RelayCommand]
    private Task OpenAsync(StudyFileItem fileItem) => _fileService.OpenAsync(fileItem);

    [RelayCommand]
    private async Task DeleteAsync(StudyFileItem fileItem)
    {
        var confirmed = await _alertService.ConfirmAsync("Delete File", "Delete this file from storage?", "Delete", "Cancel", AlertTone.Error);
        if (!confirmed)
        {
            return;
        }

        await _fileService.DeleteAsync(fileItem);
        Files.Remove(fileItem);
    }
}
