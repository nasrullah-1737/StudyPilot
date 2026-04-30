using CommunityToolkit.Mvvm.Input;
using StudyPilot.Models;
using StudyPilot.Services.Interfaces;
using System.Collections.ObjectModel;

namespace StudyPilot.ViewModels;

public partial class FilesViewModel : BaseViewModel
{
    private readonly IFileService _fileService;

    public ObservableCollection<StudyFileItem> Files { get; } = [];

    public FilesViewModel(IFileService fileService)
    {
        _fileService = fileService;
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
        var confirmed = await Shell.Current.DisplayAlert("Delete File", "Delete this file from storage?", "Delete", "Cancel");
        if (!confirmed)
        {
            return;
        }

        await _fileService.DeleteAsync(fileItem);
        Files.Remove(fileItem);
    }
}
