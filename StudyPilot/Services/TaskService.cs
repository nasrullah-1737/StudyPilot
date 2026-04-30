using StudyPilot.Models;
using StudyPilot.Services.Interfaces;

namespace StudyPilot.Services;

public class TaskService : ITaskService
{
    private readonly IDatabaseService _databaseService;
    private readonly INotificationService _notificationService;

    public TaskService(IDatabaseService databaseService, INotificationService notificationService)
    {
        _databaseService = databaseService;
        _notificationService = notificationService;
    }

    public async Task<IReadOnlyList<TaskItem>> GetAllAsync()
    {
        await _databaseService.InitializeAsync();
        return await _databaseService.Connection.Table<TaskItem>()
            .OrderBy(x => x.IsCompleted)
            .ThenBy(x => x.Deadline)
            .ToListAsync();
    }

    public async Task<TaskItem?> GetByIdAsync(int id)
    {
        await _databaseService.InitializeAsync();
        return await _databaseService.Connection.FindAsync<TaskItem>(id);
    }

    public async Task<int> SaveAsync(TaskItem taskItem)
    {
        await _databaseService.InitializeAsync();
        int changes;
        if (taskItem.Id == 0)
        {
            taskItem.CreatedAt = DateTime.UtcNow;
            changes = await _databaseService.Connection.InsertAsync(taskItem);
        }
        else
        {
            changes = await _databaseService.Connection.UpdateAsync(taskItem);
        }

        if (!taskItem.IsCompleted && taskItem.Deadline > DateTime.Now)
        {
            await _notificationService.ScheduleTaskReminderAsync(taskItem);
        }
        else
        {
            await _notificationService.CancelTaskReminderAsync(taskItem.Id);
        }

        return changes;
    }

    public async Task DeleteAsync(TaskItem taskItem)
    {
        await _databaseService.InitializeAsync();
        await _databaseService.Connection.DeleteAsync(taskItem);
        await _notificationService.CancelTaskReminderAsync(taskItem.Id);
    }

    public async Task ToggleCompletedAsync(TaskItem taskItem, bool isCompleted)
    {
        taskItem.IsCompleted = isCompleted;
        taskItem.CompletedAt = isCompleted ? DateTime.UtcNow : null;
        await SaveAsync(taskItem);
    }
}
