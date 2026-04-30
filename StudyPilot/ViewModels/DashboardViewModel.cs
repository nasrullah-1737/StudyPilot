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

    [ObservableProperty]
    private string welcomeText = "Welcome";

    [ObservableProperty]
    private int pendingTasksCount;

    [ObservableProperty]
    private int completedTasksCount;

    [ObservableProperty]
    private int todayClassesCount;

    public ObservableCollection<TaskItem> TodayTasks { get; } = [];
    public ObservableCollection<ClassScheduleItem> TodayClasses { get; } = [];

    public DashboardViewModel(
        IAuthService authService,
        ITaskService taskService,
        IScheduleService scheduleService)
    {
        _authService = authService;
        _taskService = taskService;
        _scheduleService = scheduleService;
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
            WelcomeText = user is null ? "Welcome" : $"Welcome, {user.Name}";

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
        }
        finally
        {
            IsBusy = false;
        }
    }

    [RelayCommand]
    private async Task LogoutAsync()
    {
        await _authService.LogoutAsync();
        await Shell.Current.GoToAsync(AppRoutes.LoginRoot);
    }
}
