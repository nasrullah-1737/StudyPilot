using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using StudyPilot.Models;
using StudyPilot.Services.Interfaces;

namespace StudyPilot.ViewModels;

public partial class NoteEditorViewModel : BaseViewModel, IQueryAttributable
{
    private readonly INoteService _noteService;
    private int _noteId;

    [ObservableProperty]
    private string pageTitle = "New Note";

    [ObservableProperty]
    private string noteTitle = string.Empty;

    [ObservableProperty]
    private string content = string.Empty;

    [ObservableProperty]
    private string subject = "General";

    [ObservableProperty]
    private string? imagePath;

    public NoteEditorViewModel(INoteService noteService)
    {
        _noteService = noteService;
        Title = "Note Editor";
    }

    public async void ApplyQueryAttributes(IDictionary<string, object> query)
    {
        _noteId = 0;
        if (query.TryGetValue("noteId", out var rawId) && int.TryParse(rawId?.ToString(), out var noteId))
        {
            _noteId = noteId;
        }

        if (_noteId <= 0)
        {
            PageTitle = "New Note";
            NoteTitle = string.Empty;
            Content = string.Empty;
            Subject = "General";
            ImagePath = null;
            return;
        }

        var note = await _noteService.GetByIdAsync(_noteId);
        if (note is null)
        {
            return;
        }

        PageTitle = "Edit Note";
        NoteTitle = note.Title;
        Content = note.Content;
        Subject = note.Subject;
        ImagePath = note.ImagePath;
    }

    [RelayCommand]
    private async Task PickImageAsync()
    {
        var options = new PickOptions
        {
            PickerTitle = "Pick note image",
            FileTypes = FilePickerFileType.Images
        };

        var picked = await FilePicker.Default.PickAsync(options);
        if (picked is null)
        {
            return;
        }

        var imageDir = Path.Combine(FileSystem.AppDataDirectory, "note-images");
        Directory.CreateDirectory(imageDir);

        var fileName = $"{Guid.NewGuid()}_{picked.FileName}";
        var destination = Path.Combine(imageDir, fileName);

        await using var source = await picked.OpenReadAsync();
        await using var target = File.Create(destination);
        await source.CopyToAsync(target);

        ImagePath = destination;
    }

    [RelayCommand]
    private void RemoveImage() => ImagePath = null;

    [RelayCommand]
    private async Task SaveAsync()
    {
        if (string.IsNullOrWhiteSpace(NoteTitle))
        {
            await Shell.Current.DisplayAlert("Validation", "Title is required.", "OK");
            return;
        }

        var note = _noteId > 0
            ? await _noteService.GetByIdAsync(_noteId) ?? new NoteItem()
            : new NoteItem();

        note.Title = NoteTitle.Trim();
        note.Content = Content.Trim();
        note.Subject = string.IsNullOrWhiteSpace(Subject) ? "General" : Subject.Trim();
        note.ImagePath = ImagePath;

        await _noteService.SaveAsync(note);
        await Shell.Current.GoToAsync("..");
    }
}
