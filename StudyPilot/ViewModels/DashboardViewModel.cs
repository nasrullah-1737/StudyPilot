using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using StudyPilot.Helpers;
using StudyPilot.Models;
using StudyPilot.Services.Interfaces;
using System.Collections.ObjectModel;

namespace StudyPilot.ViewModels;

public partial class DashboardViewModel : BaseViewModel
{
    private readonly IAuthService _authService;
    private readonly ITaskService _taskService;
    private readonly IScheduleService _scheduleService;
    private readonly INoteService _noteService;
    private readonly IFileService _fileService;

    [ObservableProperty]
    private string welcomeText = "Welcome";

    private string _greetingSubtitle = "Ready to study smarter today?";
    public string GreetingSubtitle
    {
        get => _greetingSubtitle;
        set => SetProperty(ref _greetingSubtitle, value);
    }

    [ObservableProperty]
    private int pendingTasksCount;

    [ObservableProperty]
    private int completedTasksCount;

    [ObservableProperty]
    private int todayClassesCount;

    private int _notesCount;
    public int NotesCount
    {
        get => _notesCount;
        set => SetProperty(ref _notesCount, value);
    }

    private int _filesCount;
    public int FilesCount
    {
        get => _filesCount;
        set => SetProperty(ref _filesCount, value);
    }

    public ObservableCollection<TaskItem> TodayTasks { get; } = [];
    public ObservableCollection<ClassScheduleItem> TodayClasses { get; } = [];

    public DashboardViewModel(
        IAuthService authService,
        ITaskService taskService,
        IScheduleService scheduleService,
        INoteService noteService,
        IFileService fileService)
    {
        _authService = authService;
        _taskService = taskService;
        _scheduleService = scheduleService;
        _noteService = noteService;
        _fileService = fileService;
        Title = "Dashboard";
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
            var user = await _authService.GetCurrentUserAsync();
            WelcomeText = user is null ? "Welcome back" : $"Hello, {user.Name.Split(' ')[0]}";
            GreetingSubtitle = GetTimeBasedGreeting();

            var tasks = await _taskService.GetAllAsync();
            PendingTasksCount = tasks.Count(x => !x.IsCompleted);
            CompletedTasksCount = tasks.Count(x => x.IsCompleted);

            TodayTasks.Clear();
            foreach (var task in tasks
                .Where(x => !x.IsCompleted && x.Deadline.Date == DateTime.Today)
                .OrderBy(x => x.Deadline))
            {
                TodayTasks.Add(task);
            }

            var todayClasses = await _scheduleService.GetForTodayAsync();
            todayClasses = todayClasses.OrderBy(x => x.StartTime).ToList();

            TodayClassesCount = todayClasses.Count;
            TodayClasses.Clear();
            foreach (var classItem in todayClasses)
            {
                TodayClasses.Add(classItem);
            }

            var notes = await _noteService.GetAllAsync();
            NotesCount = notes.Count;

            var files = await _fileService.GetAllAsync();
            FilesCount = files.Count;
        }
        finally
        {
            IsBusy = false;
        }
    }

    [RelayCommand]
    private async Task NavigateToNotesAsync() => await Shell.Current.GoToAsync(AppRoutes.NotesRoot);

    [RelayCommand]
    private async Task NavigateToTasksAsync() => await Shell.Current.GoToAsync(AppRoutes.TasksRoot);

    [RelayCommand]
    private async Task NavigateToScheduleAsync() => await Shell.Current.GoToAsync(AppRoutes.ScheduleRoot);

    [RelayCommand]
    private async Task NavigateToFocusAsync() => await Shell.Current.GoToAsync(AppRoutes.FocusRoot);

    [RelayCommand]
    private async Task NavigateToFilesAsync() => await Shell.Current.GoToAsync(AppRoutes.FilesRoot);

    private static string GetTimeBasedGreeting()
    {
        var hour = DateTime.Now.Hour;
        return hour switch
        {
            < 12 => "Good morning — let's make today productive.",
            < 17 => "Good afternoon — keep the momentum going.",
            < 21 => "Good evening — time for a focused session.",
            _ => "Burning the midnight oil? You've got this."
        };
    }
}
