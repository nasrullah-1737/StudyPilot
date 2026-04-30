using Plugin.LocalNotification;
using StudyPilot.Helpers;
using StudyPilot.Models;
using StudyPilot.Services.Interfaces;

namespace StudyPilot.Services;

public class NotificationService : global::StudyPilot.Services.Interfaces.INotificationService
{
    public async Task RequestPermissionAsync()
    {
        await Task.CompletedTask;
    }

    public async Task ScheduleTaskReminderAsync(TaskItem taskItem)
    {
        if (taskItem.Id <= 0)
        {
            return;
        }

        var notifyTime = taskItem.Deadline.AddMinutes(-30);
        if (notifyTime <= DateTime.Now)
        {
            return;
        }

        var request = new NotificationRequest
        {
            NotificationId = 10000 + taskItem.Id,
            Title = "Assignment Deadline",
            Description = $"{taskItem.Title} is due at {taskItem.Deadline:g}",
            Schedule = new NotificationRequestSchedule
            {
                NotifyTime = notifyTime
            }
        };

        await LocalNotificationCenter.Current.Show(request);
    }

    public Task CancelTaskReminderAsync(int taskId)
    {
        if (taskId > 0)
        {
            LocalNotificationCenter.Current.Cancel(10000 + taskId);
        }

        return Task.CompletedTask;
    }

    public async Task ScheduleClassReminderAsync(ClassScheduleItem classItem)
    {
        if (classItem.Id <= 0)
        {
            return;
        }

        var nextClassTime = GetNextClassDateTime(classItem).AddMinutes(-10);
        if (nextClassTime <= DateTime.Now)
        {
            return;
        }

        var request = new NotificationRequest
        {
            NotificationId = 20000 + classItem.Id,
            Title = "Upcoming Class",
            Description = $"{classItem.Subject} starts at {classItem.StartTime:hh\\:mm}",
            Schedule = new NotificationRequestSchedule
            {
                NotifyTime = nextClassTime
            }
        };

        await LocalNotificationCenter.Current.Show(request);
    }

    private static DateTime GetNextClassDateTime(ClassScheduleItem classItem)
    {
        var now = DateTime.Now;
        var todayNumber = DateHelper.ToAppDayOfWeek(now);
        var daysUntil = classItem.DayOfWeekNumber - todayNumber;
        if (daysUntil < 0)
        {
            daysUntil += 7;
        }

        var nextDate = now.Date.AddDays(daysUntil);
        var nextDateTime = nextDate + classItem.StartTime;
        if (nextDateTime <= now)
        {
            nextDateTime = nextDateTime.AddDays(7);
        }

        return nextDateTime;
    }
}
