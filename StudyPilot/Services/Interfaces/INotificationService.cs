using StudyPilot.Models;

namespace StudyPilot.Services.Interfaces;

public interface INotificationService
{
    Task RequestPermissionAsync();
    Task ScheduleTaskReminderAsync(TaskItem taskItem);
    Task CancelTaskReminderAsync(int taskId);
    Task ScheduleClassReminderAsync(ClassScheduleItem classItem);
}
