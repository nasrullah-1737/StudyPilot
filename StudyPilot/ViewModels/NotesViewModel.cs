using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using StudyPilot.Helpers;
using StudyPilot.Models;
using StudyPilot.Services.Interfaces;
using System.Collections.ObjectModel;

namespace StudyPilot.ViewModels;

public partial class NotesViewModel : BaseViewModel
{
    private readonly INoteService _noteService;
    private readonly List<NoteItem> _allNotes = [];

    [ObservableProperty]
    private string selectedSubject = "All";

    public ObservableCollection<string> Subjects { get; } = [];
    public ObservableCollection<NoteItem> Notes { get; } = [];

    public NotesViewModel(INoteService noteService)
    {
        _noteService = noteService;
        Title = "Notes";
    }

    partial void OnSelectedSubjectChanged(string value) => ApplyFilter();

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
            _allNotes.Clear();
            _allNotes.AddRange(await _noteService.GetAllAsync());

            Subjects.Clear();
            Subjects.Add("All");
            foreach (var subject in await _noteService.GetSubjectsAsync())
            {
                Subjects.Add(subject);
            }

            if (!Subjects.Contains(SelectedSubject))
            {
                SelectedSubject = "All";
            }

            ApplyFilter();
        }
        finally
        {
            IsBusy = false;
        }
    }

    [RelayCommand]
    private Task AddAsync() => Shell.Current.GoToAsync(AppRoutes.NoteEditor);

    [RelayCommand]
    private Task EditAsync(NoteItem note) => Shell.Current.GoToAsync($"{AppRoutes.NoteEditor}?noteId={note.Id}");

    [RelayCommand]
    private async Task DeleteAsync(NoteItem note)
    {
        if (note is null)
        {
            return;
        }

        var confirmed = await Shell.Current.DisplayAlert("Delete Note", "Delete this note?", "Delete", "Cancel");
        if (!confirmed)
        {
            return;
        }

        await _noteService.DeleteAsync(note);
        await LoadAsync();
    }

    private void ApplyFilter()
    {
        Notes.Clear();
        IEnumerable<NoteItem> filtered = _allNotes;
        if (!string.Equals(SelectedSubject, "All", StringComparison.OrdinalIgnoreCase))
        {
            filtered = filtered.Where(x => string.Equals(x.Subject, SelectedSubject, StringComparison.OrdinalIgnoreCase));
        }

        foreach (var note in filtered)
        {
            Notes.Add(note);
        }
    }
}
