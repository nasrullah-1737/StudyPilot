using StudyPilot.Models;

namespace StudyPilot.Services.Interfaces;

public interface ITaskService
{
    Task<IReadOnlyList<TaskItem>> GetAllAsync();
    Task<TaskItem?> GetByIdAsync(int id);
    Task<int> SaveAsync(TaskItem taskItem);
    Task DeleteAsync(TaskItem taskItem);
    Task ToggleCompletedAsync(TaskItem taskItem, bool isCompleted);
}
