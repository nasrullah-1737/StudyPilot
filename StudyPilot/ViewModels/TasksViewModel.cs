using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using StudyPilot.Helpers;
using StudyPilot.Models;
using StudyPilot.Services.Interfaces;
using System.Collections.ObjectModel;

namespace StudyPilot.ViewModels;

public partial class TasksViewModel : BaseViewModel
{
    private readonly ITaskService _taskService;
    private readonly List<TaskItem> _allTasks = [];

    [ObservableProperty]
    private TaskFilter selectedFilter = TaskFilter.All;

    public ObservableCollection<TaskFilter> Filters { get; } =
    [
        TaskFilter.All,
        TaskFilter.Pending,
        TaskFilter.Completed
    ];

    public ObservableCollection<TaskItem> Tasks { get; } = [];

    public TasksViewModel(ITaskService taskService)
    {
        _taskService = taskService;
        Title = "Assignments";
    }

    partial void OnSelectedFilterChanged(TaskFilter value) => ApplyFilter();

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
            _allTasks.Clear();
            _allTasks.AddRange(await _taskService.GetAllAsync());
            ApplyFilter();
        }
        finally
        {
            IsBusy = false;
        }
    }

    [RelayCommand]
    private Task AddAsync() => Shell.Current.GoToAsync(AppRoutes.TaskEditor);

    [RelayCommand]
    private Task EditAsync(TaskItem taskItem) => Shell.Current.GoToAsync($"{AppRoutes.TaskEditor}?taskId={taskItem.Id}");

    [RelayCommand]
    private async Task DeleteAsync(TaskItem taskItem)
    {
        if (taskItem is null)
        {
            return;
        }

        var confirmed = await Shell.Current.DisplayAlert("Delete Task", "Delete this task?", "Delete", "Cancel");
        if (!confirmed)
        {
            return;
        }

        await _taskService.DeleteAsync(taskItem);
        await LoadAsync();
    }

    [RelayCommand]
    private async Task ToggleCompletedAsync(TaskItem taskItem)
    {
        if (taskItem is null)
        {
            return;
        }

        await _taskService.ToggleCompletedAsync(taskItem, !taskItem.IsCompleted);
        await LoadAsync();
    }

    private void ApplyFilter()
    {
        Tasks.Clear();
        IEnumerable<TaskItem> filtered = _allTasks;
        switch (SelectedFilter)
        {
            case TaskFilter.Pending:
                filtered = filtered.Where(x => !x.IsCompleted);
                break;
            case TaskFilter.Completed:
                filtered = filtered.Where(x => x.IsCompleted);
                break;
        }

        foreach (var taskItem in filtered.OrderBy(x => x.IsCompleted).ThenBy(x => x.Deadline))
        {
            Tasks.Add(taskItem);
        }
    }
}
