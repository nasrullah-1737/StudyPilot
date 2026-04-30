using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using StudyPilot.Models;
using StudyPilot.Services.Interfaces;

namespace StudyPilot.ViewModels;

public partial class TaskEditorViewModel : BaseViewModel, IQueryAttributable
{
    private readonly ITaskService _taskService;
    private int _taskId;

    [ObservableProperty]
    private string pageTitle = "New Task";

    [ObservableProperty]
    private string taskTitle = string.Empty;

    [ObservableProperty]
    private string description = string.Empty;

    [ObservableProperty]
    private DateTime deadlineDate = DateTime.Today;

    [ObservableProperty]
    private TimeSpan deadlineTime = new(23, 0, 0);

    [ObservableProperty]
    private TaskPriority selectedPriority = TaskPriority.Medium;

    public IReadOnlyList<TaskPriority> Priorities { get; } =
    [
        TaskPriority.Low,
        TaskPriority.Medium,
        TaskPriority.High
    ];

    public TaskEditorViewModel(ITaskService taskService)
    {
        _taskService = taskService;
        Title = "Task Editor";
    }

    public async void ApplyQueryAttributes(IDictionary<string, object> query)
    {
        _taskId = 0;
        if (query.TryGetValue("taskId", out var rawId) && int.TryParse(rawId?.ToString(), out var taskId))
        {
            _taskId = taskId;
        }

        if (_taskId <= 0)
        {
            ResetForCreate();
            return;
        }

        var item = await _taskService.GetByIdAsync(_taskId);
        if (item is null)
        {
            ResetForCreate();
            return;
        }

        PageTitle = "Edit Task";
        TaskTitle = item.Title;
        Description = item.Description;
        DeadlineDate = item.Deadline.Date;
        DeadlineTime = item.Deadline.TimeOfDay;
        SelectedPriority = item.Priority;
    }

    [RelayCommand]
    private async Task SaveAsync()
    {
        if (string.IsNullOrWhiteSpace(TaskTitle))
        {
            await Shell.Current.DisplayAlert("Validation", "Task title is required.", "OK");
            return;
        }

        var deadline = DeadlineDate.Date + DeadlineTime;

        var item = _taskId > 0
            ? await _taskService.GetByIdAsync(_taskId) ?? new TaskItem()
            : new TaskItem();

        item.Title = TaskTitle.Trim();
        item.Description = Description.Trim();
        item.Deadline = deadline;
        item.Priority = SelectedPriority;

        await _taskService.SaveAsync(item);
        await Shell.Current.GoToAsync("..");
    }

    private void ResetForCreate()
    {
        PageTitle = "New Task";
        TaskTitle = string.Empty;
        Description = string.Empty;
        DeadlineDate = DateTime.Today;
        DeadlineTime = new TimeSpan(23, 0, 0);
        SelectedPriority = TaskPriority.Medium;
    }
}
